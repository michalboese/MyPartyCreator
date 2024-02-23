namespace PartyCreatorWebApi.Entities
{
    public class Choice
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
    }
}
