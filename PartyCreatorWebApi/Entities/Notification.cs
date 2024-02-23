namespace PartyCreatorWebApi.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int EventId { get; set; }
    }
}
