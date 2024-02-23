namespace PartyCreatorWebApi.Entities
{
    public class Gallery
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Image { get; set; } = string.Empty;

    }
}
