namespace PartyCreatorWebApi.Entities
{
    public class ShoppingListItem
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }

    }
}
