namespace PartyCreatorWebApi.Dtos
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
