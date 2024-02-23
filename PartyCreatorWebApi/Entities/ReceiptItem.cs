namespace PartyCreatorWebApi.Entities
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
