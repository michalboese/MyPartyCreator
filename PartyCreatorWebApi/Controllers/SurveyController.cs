using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Extensions;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUsersRepository _usersRepository;

        public SurveyController(ISurveyRepository surveyRepository, IEventRepository eventRepository, IUsersRepository usersRepository)
        {
            _surveyRepository = surveyRepository;
            _eventRepository = eventRepository;
            _usersRepository = usersRepository;
        }

        [HttpPost("addSurvey"), Authorize]
        public async Task<ActionResult<SurveyDto>> AddSurvey([FromBody] SurveyDto surveyDto)
        {
            var _event = await _eventRepository.GetEventDetails(surveyDto.EventId);
            if(_event == null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }

            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            if(userId != _event.CreatorId)
            {
                return BadRequest("Musisz byc administratorem wydarzenia zeby dodac ankiete");
            }

            var result = await _surveyRepository.CreateSurvey(surveyDto);
            if(result == null)
            {
                return BadRequest("Cos poszlo nie tak");
            }
            return Ok(result);
        }

        [HttpGet("getAllSurveysOfEvent/{eventId:int}"), Authorize]
        public async Task<ActionResult<List<SurveyDto>>> GetAllSurveysOfEvent(int eventId)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var checkGuest = await _eventRepository.CheckGuestList(new GuestList { Id = 0, EventId = eventId, UserId = userId });
            var eventDetails = await _eventRepository.GetEventDetails(eventId);
            if (checkGuest == null && userId != eventDetails.CreatorId)
            {
                return BadRequest("Nie uczestniczysz w tym wydarzeniu");
            }

            var result = await _surveyRepository.GetAllSurveysOfEvent(eventId);
            return Ok(result);
        }

        [HttpPost("addChoice"), Authorize]
        public async Task<ActionResult<Choice>> AddChoice([FromBody]Choice choice)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var survey = await _surveyRepository.GetSurvey(choice.SurveyId);
            if(survey == null)
            {
                return BadRequest("Nie ma takiej ankiety");
            }

            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if(userId != eventDetails.CreatorId)
            {
                return BadRequest("Musisz byc administratorem wydarzenia zeby wykonac ta czynnosc");
            }

            var result = await _surveyRepository.AddChoice(choice);
            if (result == null)
            {
                return BadRequest("Cos poszlo nie tak");
            }
            return Ok(result);
        }

        [HttpDelete("removeChoice/{id:int}"),Authorize]
        public async Task<ActionResult<Choice>> RemoveChoice(int id)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var choice = await _surveyRepository.GetChoice(id);
            if(choice == null)
            {
                return BadRequest("Nie ma takiego wyboru w ankiecie");
            }

            var survey = await _surveyRepository.GetSurvey(choice.SurveyId);

            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if (userId != eventDetails.CreatorId)
            {
                return BadRequest("Musisz byc administratorem wydarzenia zeby wykonac ta czynnosc");
            }

            var result = await _surveyRepository.RemoveChoice(id);
            if(result == null)
            {
                return BadRequest("Cos poszlo nie tak");
            }
            return Ok(result);
        }

        [HttpGet("getVotesFromSurvey/{surveyId:int}"), Authorize]
        public async Task<ActionResult<List<SurveyVoteDto>>> GetVotesFromSurvey(int surveyId)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var survey = await _surveyRepository.GetSurvey(surveyId);
            if(survey == null)
            {
                return BadRequest("Nie ma takiej ankiety");
            }

            var checkGuest = await _eventRepository.CheckGuestList(new GuestList { Id = 0, EventId = survey.EventId, UserId = userId });
            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if (checkGuest == null && userId != eventDetails.CreatorId)
            {
                return BadRequest("Nie uczestniczysz w tym wydarzeniu");
            }

            var result = await _surveyRepository.GetAllVotesOfSurvey(surveyId);
            return Ok(result);
        }

        [HttpPost("addVote"), Authorize]
        public async Task<ActionResult<SurveyVoteDto>> AddVote([FromBody] SurveyVote surveyVote)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            if(userId != surveyVote.UserId)
            {
                return BadRequest("Nie jestes osoba oddajaca glos");
            }
            
            var survey = await _surveyRepository.GetSurvey(surveyVote.SurveyId);
            if (survey == null)
            {
                return BadRequest("Nie ma takiej ankiety");
            }

            var choice = await _surveyRepository.CheckVoteChoice(surveyVote);
            if(choice == null)
            {
                return BadRequest("Nie ma takiej opcji w ankiecie");
            }

            var checkGuest = await _eventRepository.CheckGuestList(new GuestList { Id = 0, EventId = survey.EventId, UserId = userId });
            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if (checkGuest == null && userId != eventDetails.CreatorId)
            {
                return BadRequest("Nie uczestniczysz w tym wydarzeniu");
            }

            var checkIfVotedAlready = await _surveyRepository.CheckIfAlreadyVotedInSurvey(surveyVote);
            if(checkIfVotedAlready != null)
            {
                return BadRequest("Aby oddac nowy glos, trzeba usunac stary");
            }

            var result = await _surveyRepository.AddVote(surveyVote);
            if (result == null)
            {
                return BadRequest("Cos poszlo nie tak");
            }

            var user = await _usersRepository.GetUserById(userId);
            return Ok(DtoConversions.SurveyVoteToDto(surveyVote, user));
        }

        [HttpDelete("removeVote/{voteId:int}"),Authorize]
        public async Task<ActionResult<SurveyVoteDto>> RemoveVote(int voteId)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var vote = await _surveyRepository.GetVote(voteId);
            if(vote == null)
            {
                return BadRequest("Nie ma takiego glosu");
            }

            if(userId != vote.UserId)
            {
                return BadRequest("Nie jestes osoba oddajaca glos");
            }

            var survey = await _surveyRepository.GetSurvey(vote.SurveyId);
            var checkGuest = await _eventRepository.CheckGuestList(new GuestList { Id = 0, EventId = survey.EventId, UserId = userId });
            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if (checkGuest == null && userId != eventDetails.CreatorId)
            {
                return BadRequest("Nie uczestniczysz w tym wydarzeniu");
            }

            var result = await _surveyRepository.RemoveVote(voteId);
            if (result == null)
            {
                return BadRequest("Cos poszlo nie tak");
            }

            var user = await _usersRepository.GetUserById(userId);
            return Ok(DtoConversions.SurveyVoteToDto(result, user));
        }

        [HttpDelete("deleteSurvey/{surveyId:int}"), Authorize]
        public async Task<ActionResult<SurveyDto>> DeleteSurvey(int surveyId)
        {
            int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var survey = await _surveyRepository.GetSurvey(surveyId);
            if (survey == null)
            {
                return BadRequest("Nie ma takiej ankiety");
            }

            var eventDetails = await _eventRepository.GetEventDetails(survey.EventId);
            if (userId != eventDetails.CreatorId)
            {
                return BadRequest("Musisz byc administratorem wydarzenia zeby wykonac ta czynnosc");
            }

            var result = await _surveyRepository.DeleteSurvey(surveyId);
            return Ok(new SurveyDto { SurveyId = surveyId, EventId = survey.EventId, Question=survey.Question });
        }
    }
}
