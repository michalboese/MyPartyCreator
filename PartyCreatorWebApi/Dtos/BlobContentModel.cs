namespace PartyCreatorWebApi.Dtos
{
    public class BlobContentModel
    {
        public IFormFile File { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
    }
}
