namespace PartyCreatorWebApi.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
    }
}
