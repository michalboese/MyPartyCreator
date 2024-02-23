using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;
using System.Linq;

namespace PartyCreatorWebApi.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly DataContext _dataContext;

        public SurveyRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<SurveyDto> CreateSurvey(SurveyDto surveyDto)
        {
            Survey survey = new Survey()
            {
                Id = surveyDto.SurveyId,
                EventId = surveyDto.EventId,
                Question = surveyDto.Question
            };

            var result = _dataContext.Surveys.AddAsync(survey);
            await _dataContext.SaveChangesAsync();

            var surveyId = result.Result.Entity.Id;

            foreach (var item in surveyDto.Choices)
            {
                item.SurveyId = surveyId;
                _dataContext.Choices.Add(item);
            }

            await _dataContext.SaveChangesAsync();

            return surveyDto;
        }

        public async Task<Choice> AddChoice(Choice choice)
        {
            var result = _dataContext.Choices.AddAsync(choice);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }
        public async Task<Choice> RemoveChoice(int choiceId)
        {
            var result = await _dataContext.Choices.FirstOrDefaultAsync(x => x.Id == choiceId);
            if(result != null)
            {
                var votes = await _dataContext.SurveyVotes.Where(x => x.ChoiceId == choiceId).ToListAsync();
                foreach (var vote in votes)
                {
                    _dataContext.SurveyVotes.Remove(vote);
                }

                _dataContext.Choices.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }
        public async Task<Survey> DeleteSurvey(int id)
        {
            var votes = await _dataContext.SurveyVotes.Where(x => x.SurveyId == id).ToListAsync();
            foreach(var vote in votes)
            {
                _dataContext.SurveyVotes.Remove(vote);
            }

            var choices = await _dataContext.Choices.Where(x => x.SurveyId == id).ToListAsync();
            foreach(var choice in choices)
            {
                _dataContext.Choices.Remove(choice);
            }

            var survey = await _dataContext.Surveys.Where(x=> x.Id== id).FirstOrDefaultAsync();
            
            _dataContext.Surveys.Remove(survey);

            await _dataContext.SaveChangesAsync();

            return survey;
        }

        public async Task<SurveyVote> AddVote(SurveyVote vote)
        {
            var result = _dataContext.SurveyVotes.AddAsync(vote);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }

        public async Task<SurveyVote> RemoveVote(int voteId)
        {
            var result = await _dataContext.SurveyVotes.FirstOrDefaultAsync(x => x.Id == voteId);
            if (result != null)
            {
                _dataContext.SurveyVotes.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<List<SurveyVote>> RemoveAllVotes(int surveyId)
        {
            var votes = await _dataContext.SurveyVotes.Where(x => x.SurveyId == surveyId).ToListAsync();
            foreach (var vote in votes)
            {
                _dataContext.SurveyVotes.Remove(vote);
            }
            await _dataContext.SaveChangesAsync();
            return votes;
        }

        public async Task<List<SurveyDto>> GetAllSurveysOfEvent(int eventId)
        {
            List<SurveyDto> surveys = await _dataContext.Surveys
                .Where(x => x.EventId == eventId)
                .Select(s=> new SurveyDto
                {
                    EventId = s.EventId,
                    SurveyId = s.Id,
                    Question = s.Question,
                    Choices = _dataContext.Choices.Where(c=>c.SurveyId == s.Id).ToList()
                })
                .ToListAsync();
            
            return surveys;
        }

        public async Task<Survey> GetSurvey(int id)
        {
            var result = await _dataContext.Surveys.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<Choice> GetChoice(int choiceId)
        {
            var result = await _dataContext.Choices.FirstOrDefaultAsync(x => x.Id == choiceId);
            return result;
        }

        public async Task<List<SurveyVoteDto>> GetAllVotesOfSurvey(int surveyId)
        {
            var result = await _dataContext.SurveyVotes
                .Where(x=> x.SurveyId == surveyId)
                .Select(x=> new SurveyVoteDto
                {
                    SurveyId=x.SurveyId,
                    ChoiceId=x.ChoiceId,
                    Id=x.Id,
                    UserId=x.UserId,
                    FirstName= _dataContext.Users
                    .Where(u => u.Id == x.UserId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),
                    LastName= _dataContext.Users
                    .Where(u => u.Id == x.UserId)
                    .Select(u => u.LastName)
                    .FirstOrDefault(),
                })
                .ToListAsync();
            return result;
        }

        public async Task<SurveyVote> CheckIfAlreadyVotedInSurvey(SurveyVote surveyVote)
        {
            var result = await _dataContext.SurveyVotes.Where(x => x.UserId == surveyVote.UserId && x.SurveyId == surveyVote.SurveyId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<SurveyVote> GetVote(int voteId)
        {
            var result = await _dataContext.SurveyVotes.FirstOrDefaultAsync(x => x.Id == voteId);
            return result;
        }

        public async Task<Choice> CheckVoteChoice(SurveyVote surveyVote)
        {
            var result = await _dataContext.Choices.Where(x => x.SurveyId == surveyVote.SurveyId && x.Id == surveyVote.ChoiceId).FirstOrDefaultAsync();
            return result;
        }
    }
}
