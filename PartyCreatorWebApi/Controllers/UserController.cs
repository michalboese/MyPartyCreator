using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Extensions;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;

        public UserController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _usersRepository.GetUsers());
        }

        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            return Ok(await _usersRepository.GetUserByEmail(email));
        }

        [HttpGet("GetUserById/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _usersRepository.GetUserById(id);
            if (result == null)
            {
                return BadRequest("Nie znaleziono użytkownika");
            }
            return Ok(DtoConversions.UserToDto(result));
        }

        [HttpGet("GetMyProfile"), Authorize]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var result = await _usersRepository.GetUserById(creatorId);
            return Ok(DtoConversions.UserToDto(result));
        }

        [HttpPost("EditMyProfile"), Authorize]
        public async Task<ActionResult<UserDto>> EditUser(UserDto userDto)
        {



            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var result = await _usersRepository.GetUserById(creatorId);
            if (result.FirstName == userDto.FirstName && result.LastName == userDto.LastName && result.Birthday == userDto.Birthday && result.Description == userDto.Description && result.Image == userDto.Image)
            {
                return BadRequest("Nie wprowadzono zadnych zmian");
            }
            result.FirstName = userDto.FirstName;
            result.LastName = userDto.LastName;
            result.Birthday = userDto.Birthday;
            result.Description = userDto.Description;
            result.Image = userDto.Image;
            var editedUser = await _usersRepository.EditUser(result);
            return Ok(DtoConversions.UserToDto(editedUser));
        }

        [HttpPost("AddContact"), Authorize]
        public async Task<ActionResult<UserContact>> AddContact(UserContact request)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var mailcheck = await _usersRepository.GetContactByEmail(request.Email.ToLower(), creatorId);
            if (mailcheck != null)
            {
                return BadRequest("Uzytkownik został już dodany");
            }

            var user = await _usersRepository.GetUserById(creatorId);
            if (user.Email.ToLower() == request.Email.ToLower())
            {
                return BadRequest("Nie możesz dodać samego siebie");
            }

            UserContact userContact = new UserContact
            {
                UserId = creatorId,
                Name = request.Name,
                Email = request.Email.ToLower(),
            };
            var result = await _usersRepository.AddContact(userContact);
            return Ok(result);
        }

        [HttpGet("GetMyContacts"), Authorize]
        public async Task<ActionResult<UserContact>> ShowContacts()
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var result = await _usersRepository.ShowContacts(creatorId);
            return Ok(result);
        }

        [HttpGet("GetContactById/{id:int}"), Authorize]
        public async Task<ActionResult<UserContact>> GetContactById(int id)
        {
            var result = await _usersRepository.GetContactById(id);

            if (result == null)
            {
                return BadRequest("Nie znaleziono kontaktu");
            }
            return Ok(result);
        }

        [HttpPut("EditContact"), Authorize]
        public async Task<ActionResult<UserContact>> EditContact(UserContact request)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var user = await _usersRepository.GetUserById(creatorId);
            if (user.Email.ToLower() == request.Email.ToLower())
            {
                return BadRequest("Nie możesz dodać samego siebie");
            }


            var mailcheck = await _usersRepository.GetContactByEmail(request.Email.ToLower(), creatorId);

            if (mailcheck.Id != request.Id)
            {
                return BadRequest("Uzytkownik został już dodany");
            }

            var contact = await _usersRepository.GetContactById(request.Id);

            if (contact == null)
            {
                return BadRequest("Nie znaleziono kontaktu");
            }

            if (contact.UserId != creatorId)
            {
                return BadRequest("Nie masz uprawnien do edycji tego kontaktu");
            }

            if (request.Name == "" || request.Email == "")
            {
                return BadRequest("Proszę podać wszystkie dane");
            }

            if (request.Id != contact.Id)
            {
                return BadRequest("Id kontaktu nie zgadza sie z id w adresie url");
            }

            contact.Id = request.Id;
            contact.UserId = creatorId;
            contact.Name = request.Name;
            contact.Email = request.Email.ToLower();

            var result = await _usersRepository.EditContact(contact);
            return Ok(result);
        }

        [HttpDelete("DeleteContact/{id:int}"), Authorize]
        public async Task<ActionResult<UserContact>> DeleteContact(int id)
        {
            var result = await _usersRepository.DeleteContact(id);
            return Ok(result);
        }

        [HttpPost("GetUsersEmailContains"), Authorize]
        public async Task<ActionResult<UserContactDto>> GetUsersEmailContains(SearchEmailDto request)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var result = await _usersRepository.GetUsersEmailContains(request.Email.ToLower(), userId);

            return Ok(DtoConversions.UserToUserContactDto(result));
        }

        [HttpGet("GetUserType"), Authorize]
        public async Task<ActionResult<LoginResponseDto>> GetUserType()
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var result = await _usersRepository.GetUserType(userId);

            return Ok(new LoginResponseDto { Token=result});
        }

    }
}
