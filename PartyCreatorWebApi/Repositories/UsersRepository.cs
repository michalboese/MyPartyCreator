using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;
using System.Security.Claims;

namespace PartyCreatorWebApi.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersRepository(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<User> AddUser(User user)
        {
            var result = _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }

        public async Task<User> EditUser(User user)
        {
            var result = _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            return result;
        }

        public async Task<User> GetUserById(int id)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public string GetUserIdFromContext()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return result;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _dataContext.Users.ToListAsync();
        }

        public async Task<UserContact> AddContact(UserContact userContact)
        {
            var result = _dataContext.UserContacts.AddAsync(userContact);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }
        public async Task<List<UserContact>> ShowContacts(int userId)
        {
            //return a list of contacts for user with userId
            var result = _dataContext.UserContacts.Where(x => x.UserId == userId).ToListAsync();
            return await result;
        }

        public async Task<UserContact> EditContact(UserContact userContact)
        {
            var result = _dataContext.UserContacts.Update(userContact);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<UserContact> DeleteContact(int id)
        {
            UserContact userContact = new UserContact
            {
                Id = id
            };
            var result = _dataContext.UserContacts.Remove(userContact);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
            
        }

        public async Task<UserContact> GetContactById(int id)
        {
            var result = await _dataContext.UserContacts.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<List<User>> GetUsersEmailContains(string email, int userId)
        {
            var result = await _dataContext.Users.Where(u => u.Email.Contains(email) && u.Id != userId).ToListAsync();
            return result;
        }

        public async Task<UserContact> GetContactByEmail(string email, int userId)
        {
            var result = await _dataContext.UserContacts.Where(x => x.Email == email && x.UserId == userId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<User> GetUserByToken(string token)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x=> x.VerificationToken == token);
            return result;
        }

        public async Task<User> Verify(int id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            user.VerifiedAt = DateTime.Now;

            await _dataContext.SaveChangesAsync();
            return user;
        }

        public async Task<string> GetUserType(int userId)
        {
            var result = await _dataContext.Users.FirstOrDefaultAsync(x=>x.Id == userId);
            return result.Type;
        }
    }
}
