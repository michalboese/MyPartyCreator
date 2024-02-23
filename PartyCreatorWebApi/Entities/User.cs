using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace PartyCreatorWebApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Birthday { get; set; } =  DateTime.Today;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = "avatar1.png";
        public byte[] PasswordHash { get; set; } = new byte[256];
        public byte[] PasswordSalt { get; set; } = new byte[256];
        public string? VerificationToken {  get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string Type {  get; set; } = string.Empty;

    }
}
