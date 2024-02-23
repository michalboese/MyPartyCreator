using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Dtos
{
    public class SurveyDto
    {
        public int SurveyId { get; set; }
        public int EventId { get; set; }
        public string Question { get; set; } = string.Empty;
        public List<Choice> Choices { get; set; } = new List<Choice>();
    }
}
