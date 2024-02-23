namespace PartyCreatorWebApi.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthday { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = "avatar1.png";
    }
}
