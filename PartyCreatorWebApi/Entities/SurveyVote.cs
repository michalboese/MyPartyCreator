namespace PartyCreatorWebApi.Entities
{
    public class SurveyVote
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public int ChoiceId { get; set; }
    }
}
