using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface IAuthRepository
    {
        string CreateToken(User user);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateRandomToken();
    }
}
