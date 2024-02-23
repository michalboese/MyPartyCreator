namespace PartyCreatorWebApi.Entities
{
    public class Song
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string SpotifyId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        
    }
}
