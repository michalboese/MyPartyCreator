using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface IUsersRepository
    {
        Task<List<User>> GetUsers(); 
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByToken(string token);
        Task<User> Verify(int id);
        Task<User> GetUserById(int id);
        Task<User> AddUser(User user);
        Task<User> EditUser(User user);
        Task<UserContact> AddContact(UserContact userContact);
        Task<List<UserContact>> ShowContacts(int userId);
        Task<UserContact> GetContactById(int id);
        Task<UserContact> GetContactByEmail(string email, int userId);
        Task<UserContact> EditContact(UserContact userContact);
        Task<UserContact> DeleteContact(int id);
        string GetUserIdFromContext();
        Task<List<User>> GetUsersEmailContains(string email, int userId);
        Task<string> GetUserType(int userId);

    }
}
