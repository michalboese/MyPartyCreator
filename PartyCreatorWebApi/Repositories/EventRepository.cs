using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _dataContext;

        public EventRepository(DataContext dataContext)
        {
            _dataContext = dataContext;

        }

        public async Task<Event> CreateEvent(Event _event)
        {
            var result = _dataContext.Events.AddAsync(_event);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }

        public async Task<List<EventUserDto>> ListEventsJoinedByUser(int userId)
        {
            var result = await _dataContext.Events
                .Where(e => _dataContext.GuestLists.Any(gl => gl.UserId == userId && gl.EventId == e.Id))
                .Select(e => new EventUserDto
                {
                    Id = e.Id,
                    CreatorId = e.CreatorId,
                    CreatorFirstName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault(),
                    CreatorLastName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.LastName)
                        .FirstOrDefault(),
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    City = e.City,
                    Address = e.Address,
                    Country = e.Country,
                    Color = e.Color,
                    PlaylistTitle = e.PlaylistTitle,
                    ShoppingListTitle = e.ShoppingListTitle,
                    ReceiptTitle = e.ReceiptTitle
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<EventUserDto>> ListEventsMadeByUser(int creatorId)
        {
            var result = await _dataContext.Events
            .Where(x=> x.CreatorId == creatorId)
            .Select(e => new EventUserDto
            {
                Id = e.Id,
                CreatorId = e.CreatorId,
                CreatorFirstName = _dataContext.Users
                    .Where(u => u.Id == e.CreatorId)
                    .Select(u => u.FirstName)
                    .FirstOrDefault(),
                CreatorLastName = _dataContext.Users
                    .Where(u => u.Id == e.CreatorId)
                    .Select(u => u.LastName)
                    .FirstOrDefault(),
                Title = e.Title,
                Description = e.Description,
                DateTime = e.DateTime,
                City = e.City,
                Address = e.Address,
                Country = e.Country,
                Color = e.Color,
                PlaylistTitle = e.PlaylistTitle,
                ShoppingListTitle = e.ShoppingListTitle,
                ReceiptTitle = e.ReceiptTitle
            })
            .ToListAsync();

            return result;
        }

        public async Task<Event> GetEventDetails(int id)
        {
            var eventDetails = await _dataContext.Events.FirstOrDefaultAsync(e => e.Id == id);
            return eventDetails;
        }

        public async Task<InviteList> InviteToEvent(InviteList inviteList)
        {
            var result = _dataContext.InviteLists.AddAsync(inviteList);
            await _dataContext.SaveChangesAsync();
            return result.Result.Entity;
        }

        public async Task<InviteList> CheckInviteList(InviteList inviteList)
        {
            var result = await _dataContext.InviteLists.Where(i=> i.UserId == inviteList.UserId && i.EventId == inviteList.EventId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<GuestList> CheckGuestList(GuestList guestList)
        {
            var result = await _dataContext.GuestLists.Where(i => i.UserId == guestList.UserId && i.EventId == guestList.EventId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Event> DeleteEvent(int eventId)
        {
            var result = await _dataContext.Events.FirstOrDefaultAsync(x=>x.Id == eventId);
            if (result != null)
            {
                _dataContext.Events.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }
        public async Task<InviteList> DeleteInviteList(int id)
        {
            var result = await _dataContext.InviteLists.FirstOrDefaultAsync(i => i.Id == id);
            if(result != null)
            {
                _dataContext.InviteLists.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<List<InviteList>> DeleteAllInviteList(int eventId)
        {
            var result = await _dataContext.InviteLists.Where(x=>x.EventId == eventId).ToListAsync();
            foreach(var item in result)
            {
                _dataContext.InviteLists.Remove(item);
            }
            await _dataContext.SaveChangesAsync();
            return result;
        }

        public async Task<List<GuestList>> DeleteAllGuestList(int eventId)
        {
            var result = await _dataContext.GuestLists.Where(x => x.EventId == eventId).ToListAsync();
            foreach (var item in result)
            {
                _dataContext.GuestLists.Remove(item);
            }
            await _dataContext.SaveChangesAsync();
            return result;
        }

        public async Task<GuestList> AddToGuestList(GuestList guestList)
        {
            var result = await _dataContext.GuestLists.AddAsync(guestList);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<EventUserDto>> ListFinishedEvents(int userId)
        {
            var finishedEventsCreatedByUser = await _dataContext.Events
                .Where(e => e.CreatorId == userId && e.DateTime < DateTime.Now)
                .Select(e => new EventUserDto
                {
                    Id = e.Id,
                    CreatorId = e.CreatorId,
                    CreatorFirstName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault(),
                    CreatorLastName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.LastName)
                        .FirstOrDefault(),
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    City = e.City,
                    Address = e.Address,
                    Country = e.Country,
                    Color = e.Color,
                    PlaylistTitle = e.PlaylistTitle,
                    ShoppingListTitle = e.ShoppingListTitle,
                    ReceiptTitle = e.ReceiptTitle
                })
                .ToListAsync();


            var finishedEventsAsGuest = await _dataContext.Events
                .Where(e => _dataContext.GuestLists.Any(gl => gl.UserId == userId && gl.EventId == e.Id) && e.DateTime < DateTime.Now)
                .Select(e => new EventUserDto
                {
                    Id = e.Id,
                    CreatorId = e.CreatorId,
                    CreatorFirstName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.FirstName)
                        .FirstOrDefault(),
                    CreatorLastName = _dataContext.Users
                        .Where(u => u.Id == e.CreatorId)
                        .Select(u => u.LastName)
                        .FirstOrDefault(),
                    Title = e.Title,
                    Description = e.Description,
                    DateTime = e.DateTime,
                    City = e.City,
                    Address = e.Address,
                    Country = e.Country,
                    Color = e.Color,
                    PlaylistTitle = e.PlaylistTitle,
                    ShoppingListTitle = e.ShoppingListTitle,
                    ReceiptTitle = e.ReceiptTitle
                })
                .ToListAsync();


            var allFinishedEvents = finishedEventsCreatedByUser.Concat(finishedEventsAsGuest).ToList();

            // Sortuj wszystkie zakończone wydarzenia od najnowszego do najstarszego
            var sortedFinishedEvents = allFinishedEvents.OrderByDescending(e => e.DateTime).ToList();

            return sortedFinishedEvents;
        }

        public async Task<List<GuestList>> GetGuestsFromEvent(int id)
        {
            var result = await _dataContext.GuestLists.Where(x => x.EventId == id).ToListAsync();
            return result;
        }

        public async Task<List<AllGuestList>> GetAllGuestsList(int eventId)
        {
            var guestIdList = await _dataContext.GuestLists.Where(g => g.EventId == eventId).Select(g => g.UserId).ToListAsync();
            var inviteIdList = await _dataContext.InviteLists.Where(i => i.EventId == eventId).Select(i => i.UserId).ToListAsync();
            var guestList = await _dataContext.Users.Where(u => guestIdList.Contains(u.Id)).Select(u => new AllGuestList { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName}).ToListAsync();
            var sortedGuestList = guestList.OrderBy(g => g.LastName).ToList();
            var inviteList = await _dataContext.Users.Where(u => inviteIdList.Contains(u.Id)).Select(u => new AllGuestList { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName }).ToListAsync();
            var sortedInviteList = inviteList.OrderBy(g => g.LastName).ToList();
            sortedGuestList.AddRange(sortedInviteList);
            return sortedGuestList;
        }

        public async Task<List<AllGuestList>> GetGuestsUsers(int eventId)
        {
            var guestIdList = await _dataContext.GuestLists.Where(g => g.EventId == eventId).Select(g => g.UserId).ToListAsync();
            var guestList = await _dataContext.Users.Where(u => guestIdList.Contains(u.Id)).Select(u => new AllGuestList { Id = u.Id ,FirstName = u.FirstName, LastName = u.LastName }).ToListAsync();
            var sortedGuestList = guestList.OrderBy(g => g.LastName).ToList();
            return sortedGuestList;
        }

        public async Task<List<AllGuestList>> GetInvitedUsers(int eventId)
        {
            var inviteIdList = await _dataContext.InviteLists.Where(i => i.EventId == eventId).Select(i => i.UserId).ToListAsync();
            var inviteList = await _dataContext.Users.Where(u => inviteIdList.Contains(u.Id)).Select(u => new AllGuestList { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName }).ToListAsync();
            var sortedInviteList = inviteList.OrderBy(g => g.LastName).ToList();
            return sortedInviteList;
        }
    
        public async Task<Event> UpdateEvent(Event updatedEvent)
        {
            var existingEvent = await _dataContext.Events.FirstOrDefaultAsync(e => e.Id == updatedEvent.Id);

            if (existingEvent != null)
            {
                existingEvent.PlaylistTitle = updatedEvent.PlaylistTitle;
                existingEvent.ShoppingListTitle = updatedEvent.ShoppingListTitle;
                existingEvent.ReceiptTitle = updatedEvent.ReceiptTitle;
                existingEvent.Title = updatedEvent.Title;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.DateTime = updatedEvent.DateTime;
                existingEvent.City = updatedEvent.City;
                existingEvent.Address = updatedEvent.Address;
                existingEvent.Country = updatedEvent.Country;
                existingEvent.Color = updatedEvent.Color;

                // Opcjonalnie, w zależności od Twoich potrzeb, możesz dodać obsługę innych pól.

                await _dataContext.SaveChangesAsync();

                return existingEvent;
            }

            return null; // Zwróć null, jeśli nie udało się znaleźć wydarzenia do zaktualizowania.
        }

        public async Task AddEventFunctions(int eventId, EventFunctionsDto eventFunctions)
        {
            var existingEvent = await _dataContext.Events.FindAsync(eventId);

            if (existingEvent != null)
            {
                existingEvent.PlaylistTitle = eventFunctions.PlaylistTitle;
                existingEvent.ShoppingListTitle = eventFunctions.ShoppingListTitle;
                existingEvent.ReceiptTitle = eventFunctions.ReceiptTitle;

                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<GuestList> DeleteGuestList(int id)
        {
            var result = await _dataContext.GuestLists.FirstOrDefaultAsync(i => i.Id == id);
            if (result != null)
            {
                _dataContext.GuestLists.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }
    }
}

