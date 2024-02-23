namespace PartyCreatorWebApi.Dtos
{
    public class SurveyVoteDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int ChoiceId { get; set; }
    }
}
