using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Repositories.Contracts;
using Azure.Storage.Blobs.Models;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly DataContext _dataContext;
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient client;
        public static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".jpeg" };

        public BlobStorageRepository(BlobServiceClient blobServiceClient, DataContext dataContext)
        {
            _blobServiceClient = blobServiceClient;
            client = _blobServiceClient.GetBlobContainerClient("photos");
            _dataContext = dataContext;

        }
        public async Task<Gallery> DeleteBlobFile(int id)    
        {
            var image = await _dataContext.Galleries.FirstOrDefaultAsync(x => x.Id == id);
            if (image == null)
            {
                throw new ArgumentException("Image not found");
            }

            var fileName = new Uri(image.Image).Segments.LastOrDefault();
            if (fileName != null)
            {
                var blobClient = client.GetBlobClient(fileName);
                await blobClient.DeleteIfExistsAsync();
            }

            _dataContext.Galleries.Remove(image);
            await _dataContext.SaveChangesAsync();
            return image;
        }


        public async Task<BlobObject> GetBlobFile(string url)
        {
            var fileName = new Uri(url).Segments.LastOrDefault();

            // Early return if the filename is null or empty
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                var blobClient = client.GetBlobClient(fileName);
                var blobExists = await blobClient.ExistsAsync();

                // Early return if the blob does not exist
                if (!blobExists)
                {
                    return null;
                }

                BlobDownloadResult content = await blobClient.DownloadContentAsync();
                var downloadedData = content.Content.ToStream();

                // Determine the content type
                string contentType;
                if (AllowedExtensions.Contains(Path.GetExtension(fileName.ToLowerInvariant())))
                {
                    var extension = Path.GetExtension(fileName);
                    contentType = "image/" + extension.Replace(".", "");
                }
                else
                {
                    contentType = content.Details.ContentType;
                }

                return new BlobObject
                {
                    Content = downloadedData,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<Gallery> UploadBlobFile(IFormFile file, int eventId, int userId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be empty");
            }

            var uniqueFileName = Guid.NewGuid().ToString();
            //var fileName = $"{eventId}_{userId}_{file.FileName}_{uniqueFileName}"; //{Path.GetFileName(file.FileName)}

            var blobClient = client.GetBlobClient(uniqueFileName);
            
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            string fileUrl = blobClient.Uri.AbsoluteUri;

            Gallery gallery = new()
            {
                EventId = eventId,
                UserId = userId,
                Image = fileUrl
            };
            var result = await _dataContext.Galleries.AddAsync(gallery);
            await _dataContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<List<Gallery>> GetImageByEventId(int eventId)
        {
            var result = await _dataContext.Galleries.Where(x => x.EventId == eventId).ToListAsync();
          
            return result;
        }
        public async Task<Gallery> GetImageById(int id)
        {
            var result = await _dataContext.Galleries.FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }
    }
}
