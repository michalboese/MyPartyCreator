using Microsoft.AspNetCore.Mvc;
using PartyCreatorWebApi.Repositories.Contracts;
using PartyCreatorWebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration.UserSecrets;
using PartyCreatorWebApi.Dtos;

namespace PartyCreatorWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IEventRepository _eventRepository;

        public ShoppingListController(IShoppingListRepository shoppingListRepository, IUsersRepository usersRepository, IEventRepository eventRepository)
        {
            _shoppingListRepository = shoppingListRepository;
            _usersRepository = usersRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet("GetShoppingList/{eventId:int}"), Authorize]
        public async Task<ActionResult<List<ShoppingListItem>>> GetShopingList(int eventId)
        {
            var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var isEvent = await _eventRepository.GetEventDetails(eventId);
            if (isEvent == null)
            {
                return BadRequest("Nie ma takiego wydarzenia");
            }
            var creatorId = isEvent.CreatorId;
            
            
            GuestList guestlist = new GuestList
            {
                EventId = eventId,
                UserId = userId
            };

            var result = await _eventRepository.CheckGuestList(guestlist);
            if (result == null && userId != creatorId)
            {
                return BadRequest("Nie jesteś na liście gości");
            }

            return Ok(await _shoppingListRepository.GetShoppigList(eventId));
        }

        [HttpPost("NewShoppingListItem"), Authorize]
        public async Task<ActionResult<ShoppingListItem>> NewShoppingListItem(ShoppingListItem request)
        {
            var creatorId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var isEvent = await _eventRepository.GetEventDetails(request.EventId);
            if (isEvent == null)
            {
                return NotFound("Nie ma takiego wydarzenia");
            }
            var eventCreatorId = isEvent.CreatorId;
            if (creatorId != eventCreatorId)
            {
                return BadRequest("Musisz być twórcą wydarzenia aby dodać przedmiot");
            }

            ShoppingListItem shoppingListItem = new()
            {
                EventId = request.EventId,
                Name = request.Name,
                Quantity = request.Quantity,
            }; 

            return await _shoppingListRepository.NewShoppingListItem(shoppingListItem);
        }

        [HttpPut("SignUpForItem/{id:int}"), Authorize]
        public async Task<ActionResult<ShoppingListItem>> SignUpForItem(int id)
        {
            var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var shoppingListItem = await _shoppingListRepository.GetShoppingListItemById(id);
            if (shoppingListItem == null)
            {
                return BadRequest("Nie ma takiego przedmiotu");
            }
            if (shoppingListItem.UserId != 0)
            {
                return BadRequest("Przedmiot jest już zarezerwowany");
            }
            shoppingListItem.UserId = userId;
            return await _shoppingListRepository.UpdateShoppingListItem(shoppingListItem);
        }

        [HttpPut("SignOutFromItem/{id:int}"), Authorize]
        public async Task<ActionResult<ShoppingListItem>> SignOutFromItem(int id)
        {
            var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
            var shoppingListItem = await _shoppingListRepository.GetShoppingListItemById(id);
            var eventCreatorId = _eventRepository.GetEventDetails(shoppingListItem.EventId).Result.CreatorId;
            if (shoppingListItem == null)
            {
                return BadRequest("Nie ma takiego przedmiotu");
            }
            if (shoppingListItem.UserId != userId && userId != eventCreatorId)
            {
                return BadRequest("Nie możesz zrezygnować z przedmiotu, którego nie zarezerwowałeś");
            }
            shoppingListItem.UserId = 0;
            return await _shoppingListRepository.UpdateShoppingListItem(shoppingListItem);
        }
        [HttpDelete("RemoveShoppingListItem/{eventId:int}/{itemId:int}"), Authorize]
        public async Task<ActionResult<bool>> RemoveShoppingListItem(int eventId, int itemId)
        {
            try
            {
                var userId = Int32.Parse(_usersRepository.GetUserIdFromContext());
                var shoppingListItem = await _shoppingListRepository.GetShoppingListItemById(itemId);

                if (shoppingListItem == null)
                {
                    return NotFound("Nie ma takiego przedmiotu");
                }

                var eventCreatorId = _eventRepository.GetEventDetails(shoppingListItem.EventId).Result.CreatorId;

                // Sprawdź, czy użytkownik ma uprawnienia do usunięcia przedmiotu
                if (userId != shoppingListItem.UserId && userId != eventCreatorId)
                {
                    return Unauthorized("Nie masz uprawnień do usunięcia tego przedmiotu");
                }

                var result = await _shoppingListRepository.RemoveShoppingListItem(itemId);

                if (result)
                {
                    return Ok(true);
                }

                return BadRequest("Wystąpił błąd podczas usuwania przedmiotu");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wewnętrzny błąd serwera: {ex.Message}");
            }
        }


    }
}
