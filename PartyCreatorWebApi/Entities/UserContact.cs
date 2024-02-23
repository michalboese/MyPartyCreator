namespace PartyCreatorWebApi.Entities
{
    public class UserContact
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
}
