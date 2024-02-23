using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface ISurveyRepository
    {
        Task<SurveyDto> CreateSurvey(SurveyDto surveyDto);
        Task<Choice> AddChoice(Choice choice);
        Task<Choice> RemoveChoice(int choiceId);
        Task<Choice> GetChoice(int choiceId);
        Task<SurveyVote> AddVote(SurveyVote vote);
        Task<SurveyVote> RemoveVote(int voteId);
        Task<List<SurveyVoteDto>> GetAllVotesOfSurvey(int surveyId);
        Task<List<SurveyDto>> GetAllSurveysOfEvent(int eventId);
        Task<Survey> GetSurvey(int id);
        Task<SurveyVote> CheckIfAlreadyVotedInSurvey(SurveyVote surveyVote);
        Task<SurveyVote> GetVote(int voteId);
        Task<Choice> CheckVoteChoice(SurveyVote surveyVote);
        Task<Survey> DeleteSurvey(int id);
        Task<List<SurveyVote>> RemoveAllVotes(int surveyId);
    }
}
