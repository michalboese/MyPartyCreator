namespace PartyCreatorWebApi.Entities
{
    public class Survey
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Question { get; set; } = string.Empty;
    }
}
