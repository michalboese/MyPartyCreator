namespace PartyCreatorWebApi.Dtos
{
    public class ContactEventDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int EventId { get; set; }
    }
}
