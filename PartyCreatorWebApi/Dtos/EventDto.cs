namespace PartyCreatorWebApi.Dtos
{
    public class EventDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string PlaylistTitle { get; set; } = string.Empty;
        public string ShoppingListTitle { get; set; } = string.Empty;
        public string ReceiptTitle { get; set; } = string.Empty;
    }
}
