using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface IBlobStorageRepository
    {
        Task<BlobObject> GetBlobFile(string name);
        Task<Gallery> UploadBlobFile(IFormFile file, int eventId, int userId);
        Task<Gallery> DeleteBlobFile(int id);
        Task<List<Gallery>> GetImageByEventId(int eventId);
        Task<Gallery> GetImageById(int id);

    }
}
