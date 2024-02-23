namespace PartyCreatorWebApi.Dtos
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
