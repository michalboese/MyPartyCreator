using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;
using System.Security.Claims;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AuthController(IAuthRepository authRepository ,IUsersRepository usersRepository, IConfiguration configuration, HttpClient httpClient)
        {
            _authRepository = authRepository;
            _usersRepository = usersRepository;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        
        [HttpGet("test")]
        public string Test()
        {
            return "adamtogejestfajny";
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto request)
        {   
            //sprawdz czy email juz istnieje
            var result = await _usersRepository.GetUserByEmail(request.Email.ToLower());

            if (result != null)
            {
                return BadRequest("Użytkownik o takim email już istnieje");
            }

            //mozna jeszcze sprawdzic czy haslo jest o podanych wytycznych
            //utworz password hash i salt
            _authRepository.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Description = request.Description,
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = _authRepository.CreateRandomToken(),
                Type = "Email"
            };
            
            //dodaj uzytkownika do bazy
            var addedUser = await _usersRepository.AddUser(user);

            if(addedUser==null)
            {
                return BadRequest("Wystapil problem, nie udalo sie zarejestrowac");
            }

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("partycreator@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "PartyCreator Weryfikacja";
            email.Body = new TextPart(TextFormat.Html) { Text = $"<h1>Potwierdz email<h1><h2>https://localhost:7241/api/Auth/verify?token={user.VerificationToken}<h2>" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mariusz.bar.walce@gmail.com", "gK6B82DOqJHXQFbk");
            smtp.Send(email);
            smtp.Disconnect(true);

            UserDto userDto = new UserDto
            {
                Id = addedUser.Id,
                FirstName = addedUser.FirstName,
                LastName = addedUser.LastName,
                Description = addedUser.Description,
                Birthday = addedUser.Birthday,
                Email = addedUser.Email,
                //type email
            };

            return Ok(userDto);
        }

        [HttpGet("getme"), Authorize]
        public ActionResult<string> GetMe()
        {
            var userId = _usersRepository.GetUserIdFromContext();
            return Ok(userId);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto request)
        {
            var user = await _usersRepository.GetUserByEmail(request.Email);

            if(user == null)
            {
                return BadRequest("Nie znaleziono użytkownika");
            }

            if(user.Type == "Facebook")
            {
                return BadRequest("Musisz zalogowac sie przez Facebooka");
            }

            if(!_authRepository.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Podane haslo sie nie zgadza");
            }
            
            if(user.Type == "Email" && user.VerifiedAt == null)
            {
                return BadRequest("Musisz najpierw potwierdzic email zeby sie zalogowac");
            }

            string token = _authRepository.CreateToken(user);

            LoginResponseDto loginResponseDto = new LoginResponseDto
            {
                Token = token
            };

            return Ok(loginResponseDto);
        }

        [HttpGet("verify")]
        public async Task<ActionResult<LoginResponseDto>> Verify([FromQuery]string token)
        {
            var user = await _usersRepository.GetUserByToken(token);

            if (user == null)
            {
                return BadRequest("Niepoprawny token");
            }

            var result = await _usersRepository.Verify(user.Id);

            return Ok("Udalo sie potwierdzic email");
        }

        [HttpPost("change-password"), Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            
            var user = await _usersRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }

            
            if (!_authRepository.VerifyPasswordHash(changePasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Niepoprawne stare hasło.");
            }

           
          //  if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            //{
              //  return BadRequest("Nowe hasło i potwierdzenie hasła nie są takie same.");
            //}

            _authRepository.CreatePasswordHash(changePasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

            
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            var updatedUser = await _usersRepository.EditUser(user);

            if (updatedUser == null)
            {
                return BadRequest("Nie udało się zaktualizować hasła.");
            }

            return Ok();
        }

        [HttpPost("LoginWithFacebook")]
        public async Task<ActionResult> LoginWithFacebook([FromBody] string credential)
        {
            
            var facebookId = _configuration.GetSection("AppSettings:FacebookAppId").Value;

            var facebookSecret = _configuration.GetSection("AppSettings:FacebookSecret").Value;

            HttpResponseMessage debugTokenResponse = await _httpClient.GetAsync("https://graph.facebook.com/debug_token?input_token=" + credential + $"&access_token={facebookId}|{facebookSecret}");

            var stringThing = await debugTokenResponse.Content.ReadAsStringAsync();
            var userOBJK = JsonConvert.DeserializeObject<FBUser>(stringThing);

            if (userOBJK.Data.isValid == false)
            {
                return Unauthorized();
            }

            HttpResponseMessage meResponse = await _httpClient.GetAsync("https://graph.facebook.com/me?fields=first_name,last_name,email,id&access_token=" + credential);
            var userContent = await meResponse.Content.ReadAsStringAsync();
            var userContentObj = JsonConvert.DeserializeObject<FBUserInfo>(userContent);

            var user = await _usersRepository.GetUserByEmail(userContentObj.Email);
            if (user != null && user.Type=="Facebook")
            {
                string token = _authRepository.CreateToken(user);

                LoginResponseDto loginResponseDto = new LoginResponseDto
                {
                    Token = token
                };

                return Ok(loginResponseDto);
            }
            else if(user != null && user.Type=="Email")
            {
                return BadRequest("Musisz zalogowac sie przez email");
            }
            else
            {
                User newUser = new User
                {
                    FirstName = userContentObj.FirstName,
                    LastName = userContentObj.LastName,
                    Email = userContentObj.Email.ToLower(),
                    Type="Facebook"
                };

                //dodaj uzytkownika do bazy
                var addedUser = await _usersRepository.AddUser(newUser);

                if (addedUser == null)
                {
                    return BadRequest("Wystapil problem, nie udalo sie zarejestrowac");
                }

                string token = _authRepository.CreateToken(addedUser);

                LoginResponseDto loginResponseDto = new LoginResponseDto
                {
                    Token = token
                };

                return Ok(loginResponseDto);
            }

        }


    }
}
