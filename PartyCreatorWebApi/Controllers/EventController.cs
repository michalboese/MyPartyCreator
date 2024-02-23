using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Extensions;
using PartyCreatorWebApi.HubConfig;
using PartyCreatorWebApi.Migrations;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IBlobStorageRepository _blobStorageRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IReceiptItemRepository _receiptItemRepository;
        private readonly IChatRepository _chatRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISpotifyRepository _spotifyRepository;

        public EventController(IEventRepository eventRepository, IUsersRepository usersRepository, INotificationRepository notificationRepository, IHubContext<ChatHub> hub, IBlobStorageRepository blobStorageRepository, IShoppingListRepository shoppingListRepository, IReceiptItemRepository receiptItemRepository, IChatRepository chatRepository, ISurveyRepository surveyRepository, ISpotifyRepository spotifyRepository)
        {
            _eventRepository = eventRepository;
            _usersRepository = usersRepository;
            _notificationRepository = notificationRepository;
            _hub = hub;
            _blobStorageRepository = blobStorageRepository;
            _shoppingListRepository = shoppingListRepository;
            _receiptItemRepository = receiptItemRepository;
            _chatRepository = chatRepository;
            _surveyRepository = surveyRepository;
            _spotifyRepository = spotifyRepository;
        }

        [HttpGet("getOfCreator"), Authorize]
        public async Task<ActionResult<List<Event>>> GetOfCreator()
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var result = await _eventRepository.ListEventsMadeByUser(creatorId);
            return Ok(result);
        }

        [HttpPost("create"), Authorize]
        public async Task<ActionResult<Event>> Create(EventDto request)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            Event _event = new Event
            {
                CreatorId = creatorId,
                PlaylistTitle = request.PlaylistTitle,
                ShoppingListTitle = request.ShoppingListTitle,
                ReceiptTitle = request.ReceiptTitle,
                Title = request.Title,
                Description = request.Description,
                DateTime = request.DateTime,
                City = request.City,
                Address = request.Address,
                Country = request.Country,
                Color = request.Color
            };

            var addedEvent = await _eventRepository.CreateEvent(_event);

            if (addedEvent == null)
            {
                return BadRequest("Wystapil problem, nie udalo sie stworzyć wydarzenia");
            }

            return Ok(addedEvent);
        }
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<EventUserDto>> GetEventDetails(int id)
        {
            var eventDetails = await _eventRepository.GetEventDetails(id);

            if (eventDetails == null)
            {
                return NotFound("Nie znaleziono wydarzenia o podanym ID");
            }
            var user = await _usersRepository.GetUserById(eventDetails.CreatorId);
            

            return Ok(DtoConversions.EventToDto(eventDetails, user));
        }
        [HttpGet("getUpcoming"), Authorize]
        public async Task<ActionResult<List<EventUserDto>>> GetUpcomingEvents()
        {
            try
            {
                int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());


                var upcomingEvents = await _eventRepository.ListEventsJoinedByUser(userId);
                return Ok(upcomingEvents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("invite"), Authorize]
        public async Task<ActionResult<InviteList>> InviteToEvent(InviteList request)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            if(request.UserId == creatorId)
            {
                return BadRequest("Nie możesz zaprosić samego siebie do wydarzenia");
            }

            var user = await _usersRepository.GetUserById(request.UserId);
            if(user == null)
            {
                return BadRequest("Nie znaleziono takiego użytkownika");
            }

            var _event = await _eventRepository.GetEventDetails(request.EventId);
            if(_event == null)
            {
                return BadRequest("Nie znaleziono takiego wydarzenia");
            }

            if(creatorId != _event.CreatorId)
            {
                return BadRequest("Musisz być twórcą wydarzenia aby móc do niego zapraszać");
            }

            var invitedUser = await _eventRepository.CheckInviteList(request);
            if(invitedUser != null)
            {
                return BadRequest("Użytkownik jest już zaproszony do wydarzenia");
            }

            var guestList = new GuestList
            {
                UserId = request.UserId,
                EventId = request.EventId
            };
            var guestUser = await _eventRepository.CheckGuestList(guestList);
            if(guestUser != null)
            {
                return BadRequest("Uzytkownik już uczestniczy w wydarzeniu");
            }

            var result = await _eventRepository.InviteToEvent(request);
            if(result == null)
            {
                return BadRequest("Wystąpił błąd podczas zapraszania");
            }

            var notification = new Notification
            {
                UserId = request.UserId,
                Description = "Zaproszenie do wydarzenia",
                Type = "Zaproszenie",
                IsRead = false,
                EventId = request.EventId
            };

            var notificationDto = await _notificationRepository.CreateNotification(notification);
            await _hub.Clients.User(request.UserId.ToString()).SendAsync("ReceiveNotification", notificationDto);

            return Ok(result);
        }

        [HttpPost("accept"), Authorize]
        public async Task<ActionResult<GuestList>> AcceptInvite(Notification request) //tu mozna zmienic notification
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            if(request.UserId != creatorId)
            {
                return BadRequest("Możesz akceptować tylko twoje wydarzenia");
            }

            var _event = await _eventRepository.GetEventDetails(request.EventId);
            if(_event == null)
            {
                return BadRequest("Nie można znaleźć wydarzenia");
            }

            //usun invite
            var invite = await _eventRepository.CheckInviteList(new InviteList { UserId = request.UserId, EventId = request.EventId });
            if(invite == null)
            {
                return BadRequest("Nie masz zaproszenia");
            }
            await _eventRepository.DeleteInviteList(invite.Id);
            //dodaj do guest
            var guest = await _eventRepository.AddToGuestList(new GuestList { UserId=request.UserId, EventId = request.EventId });
            if(guest == null)
            {
                return BadRequest("Nie udało się dodać do wydarzenia");
            }
            //usun powiadomienie
            await _notificationRepository.DeleteNotification(request.Id);
            ChangeGuestInviteDto changeGuestInviteDto = new ChangeGuestInviteDto { GuestListId = guest.Id, InviteListId = invite.Id };
            await _hub.Clients.Group(_event.Id.ToString()).SendAsync("AcceptedInvite", changeGuestInviteDto);

            return Ok(guest);
        }

        [HttpPost("decline"), Authorize]
        public async Task<ActionResult<InviteList>> DeclineIvite(Notification request)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            if (request.UserId != creatorId)
            {
                return BadRequest("Możesz akceptować tylko twoje wydarzenia");
            }

            var _event = await _eventRepository.GetEventDetails(request.EventId);
            if (_event == null)
            {
                return BadRequest("Nie można znaleźć wydarzenia");
            }

            //usun invite
            var invite = await _eventRepository.CheckInviteList(new InviteList { UserId = request.UserId, EventId = request.EventId });
            if (invite == null)
            {
                return BadRequest("Nie masz zaproszenia");
            }
            var result = await _eventRepository.DeleteInviteList(invite.Id);
            
            //usun powiadomienie
            await _notificationRepository.DeleteNotification(request.Id);
            
            await _hub.Clients.Group(_event.Id.ToString()).SendAsync("DeclineInvite", invite);
            return Ok(result);

        }

        [HttpGet("getFinished"), Authorize]
        public async Task<ActionResult<List<Event>>> GetFinishedEvents()
        {
            try
            {
                int userId = Int32.Parse(_usersRepository.GetUserIdFromContext());

                // Pobierz wszystkie zakończone wydarzenia dla danego użytkownika
                var finishedEvents = await _eventRepository.ListFinishedEvents(userId);

                return Ok(finishedEvents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("getAccess/{id}"), Authorize]
        public async Task<ActionResult<RoleDto>> GetAccess(int id)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var _event = await _eventRepository.GetEventDetails(id);
            if(_event==null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }

            if(creatorId == _event.CreatorId)
            {
                return Ok(new RoleDto { Id = creatorId, Role = "Admin" });
            }

            var guests = await _eventRepository.GetGuestsFromEvent(id);
            var user = guests.Where(x => x.UserId == creatorId).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Nie jesteś na liście wydarzenia");
            }
            return Ok(new RoleDto { Id = creatorId, Role = "Guest" });
        }

        [HttpGet("GetAllGuests/{id:int}"), Authorize]
        public async Task<ActionResult<List<AllGuestList>>> GetAllGuests(int id)
        {
            var result = await _eventRepository.GetAllGuestsList(id);
            return Ok(result);
        }

        [HttpGet("GetGuestsUsers/{id:int}"), Authorize]
        public async Task<ActionResult<List<AllGuestList>>> GetGuestsUsers(int id)
        {
            var result = await _eventRepository.GetGuestsUsers(id);
            return Ok(result);
        }

        [HttpGet("GetInvitesUsers/{id:int}"), Authorize]
        public async Task<ActionResult<List<AllGuestList>>> GetInvitesUsers(int id)
        {
            var result = await _eventRepository.GetInvitedUsers(id);
            return Ok(result);
        }

        [HttpPost("inviteEmail"), Authorize]
        public async Task<ActionResult<InviteList>> InviteToEventEmail(ContactEventDto request)
        {
            var user = await _usersRepository.GetUserByEmail(request.Email);
            if(user == null)
            {
                return BadRequest("Nie ma użytkownika o takim email");
            }
            var test = new InviteList
            {
                UserId = user.Id,
                EventId = request.EventId,
            };
            
            return await InviteToEvent(test);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<ActionResult<Event>> UpdateEvent(int id, EventDto updatedEventDto)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            // Sprawdź, czy użytkownik jest twórcą wydarzenia
            var existingEvent = await _eventRepository.GetEventDetails(id);
            if (existingEvent == null)
            {
                return NotFound("Nie znaleziono wydarzenia o podanym ID");
            }

            if (creatorId != existingEvent.CreatorId)
            {
                return Unauthorized("Nie jesteś uprawniony do aktualizacji tego wydarzenia");
            }

            existingEvent.Title = updatedEventDto.Title;
            existingEvent.Description = updatedEventDto.Description;
            existingEvent.DateTime = updatedEventDto.DateTime;

            var updatedEvent = await _eventRepository.UpdateEvent(existingEvent);

            if (updatedEvent == null)
            {
                return BadRequest("Wystąpił problem podczas aktualizacji wydarzenia");
            }

            var notifications = await _notificationRepository.CreateNotificationToAllGuests(new Notification
            {
                Id = 0,
                Description = "Wydarzenie uległo zmianie",
                IsRead = false,
                Type = "Powiadomienie",
                EventId = id,
                UserId = 0
            });

            foreach (var notification in notifications)
            {
                await _hub.Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
            }

            return Ok(updatedEvent);
        }
        
        [HttpDelete("deleteGuest/{eventId}/{userId}"), Authorize]
        public async Task<ActionResult<GuestList>> DeleteGuest(int eventId, int userId)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());


            var _event = await _eventRepository.GetEventDetails(eventId);
            if (_event == null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }

            if (creatorId != _event.CreatorId)
            {
                return BadRequest("Nie masz dostepu do usuwania");
            }

            var guestList = await _eventRepository.CheckGuestList(new GuestList { EventId=eventId, UserId=userId, Id=0});
            if(guestList == null)
            {
                return BadRequest("Nie ma takiego uczestnika w tym wydarzeniu");
            }

            var result = await _eventRepository.DeleteGuestList(guestList.Id);

            return Ok(result);
        }

        [HttpPost("{eventId}/addFunctions")]
        public async Task<IActionResult> AddEventFunctions(int eventId, [FromBody] EventFunctionsDto eventFunctions)
        {
            try
            {
                await _eventRepository.AddEventFunctions(eventId, eventFunctions);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("deleteInvited/{eventId}/{userId}"), Authorize]
        public async Task<ActionResult<InviteList>> DeleteInvited(int eventId, int userId)
        {
            int creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var _event = await _eventRepository.GetEventDetails(eventId);
            if (_event == null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }

            if (creatorId != _event.CreatorId)
            {
                return BadRequest("Nie masz dostepu do usuwania");
            }

            var guestList = await _eventRepository.CheckInviteList(new InviteList { EventId = eventId, UserId = userId, Id = 0 });
            if (guestList == null)
            {
                return BadRequest("Nie ma takiego uczestnika w tym wydarzeniu");
            }
            //usunac powiadomienie
            var notification = await _notificationRepository.GetNotificationByUserIdEventId(userId, eventId);
            await _notificationRepository.DeleteNotification(notification.Id);

            var result = await _eventRepository.DeleteInviteList(guestList.Id);

            return Ok(result);
        }

        [HttpDelete("deleteEvent/{eventId}"), Authorize]
        public async Task<ActionResult<Event>> DeleteEvent(int eventId)
        {
            int userid = Int32.Parse(_usersRepository.GetUserIdFromContext());

            var _event = await _eventRepository.GetEventDetails(eventId);
            if(_event == null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }

            if(_event.CreatorId != userid)
            {
                return BadRequest("Nie masz uprawnien do usuwania");
            }

            //gallery
            var images = await _blobStorageRepository.GetImageByEventId(eventId);
            foreach(var image in images)
            {
                await _blobStorageRepository.DeleteBlobFile(image.Id);
            }

            //shopping list
            var shoppingList = await _shoppingListRepository.GetShoppigList(eventId);
            foreach(var list in shoppingList)
            {
                await _shoppingListRepository.RemoveShoppingListItem(list.Id);
            }

            //receipt
            var receiptitems = await _receiptItemRepository.GetReceiptItems(eventId);
            foreach(var receiptitem in receiptitems)
            {
                await _receiptItemRepository.RemoveReceiptItem(receiptitem.Id);
            }

            //notification
            await _notificationRepository.DeleteAllFromEvent(eventId);

            //survey
            var survey = await _surveyRepository.GetAllSurveysOfEvent(eventId);
            foreach (var item in survey)
            {
                await _surveyRepository.DeleteSurvey(item.SurveyId);
            }
            //guestlist
            await _eventRepository.DeleteAllGuestList(eventId);
            //invitelist
            await _eventRepository.DeleteAllInviteList(eventId);
            //chatmessage
            await _chatRepository.DeleteAllFromEvent(eventId);

            //songs
            await _spotifyRepository.DeleteAllSongs(eventId);

            //event
            var deletedEvent = await _eventRepository.DeleteEvent(eventId);

            await _hub.Clients.Group(eventId.ToString()).SendAsync("DeleteEvent");

            return Ok(deletedEvent);
        }
    }

}


 



