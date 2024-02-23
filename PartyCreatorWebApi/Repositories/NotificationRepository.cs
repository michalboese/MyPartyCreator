using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;
using PartyCreatorWebApi.Repositories.Contracts;

namespace PartyCreatorWebApi.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DataContext _dataContext;

        public NotificationRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<NotificationDto> CreateNotification(Notification request)
        {
            var result = _dataContext.Notifications.AddAsync(request);
            await _dataContext.SaveChangesAsync();
            var notification = new NotificationDto
            {
                Id = result.Result.Entity.Id,
                Description = request.Description,
                EventId = request.EventId,
                IsRead = request.IsRead,
                Type = request.Type,
                UserId = request.UserId,
                EventTitle = _dataContext.Events.Where(e => e.Id == request.EventId).Select(t => t.Title).FirstOrDefault()
            };
            return notification;
        }

        public async Task<List<NotificationDto>> CreateNotificationToAllGuests(Notification notification)
        {
            var guests = await _dataContext.GuestLists.Where(x => x.EventId == notification.EventId).ToListAsync();
            List<NotificationDto> results = new List<NotificationDto>();

            foreach(GuestList guest in  guests)
            {
                notification.UserId= guest.UserId;
                var result = await _dataContext.Notifications.AddAsync(notification);
                await _dataContext.SaveChangesAsync();

                var notificationDto = new NotificationDto
                {
                    Id = result.Entity.Id,
                    Description = notification.Description,
                    EventId = notification.EventId,
                    IsRead = notification.IsRead,
                    Type = notification.Type,
                    UserId = notification.UserId,
                    EventTitle = await _dataContext.Events.Where(e => e.Id == notification.EventId).Select(t => t.Title).FirstOrDefaultAsync()
                };

                results.Add(notificationDto);
            }
            return results;
        }

        public async Task<Notification> DeleteNotification(int id)
        {
            var result = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if(result != null)
            {
                _dataContext.Notifications.Remove(result);
                await _dataContext.SaveChangesAsync();
                return result;
            }
            return null;
        }

        public async Task<List<NotificationDto>> GetAllNotificationsOfUser(int id)
        {
            var result = await _dataContext.Notifications.Where(n => n.UserId == id)
                .Select(n=>new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Description = n.Description,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    EventId = n.EventId,
                    EventTitle = _dataContext.Events.Where(e=>e.Id==n.EventId).Select(u=>u.Title).FirstOrDefault(),
                }).ToListAsync();
            
            return result;
        }

        public async Task<Notification> GetNotification(int id)
        {
            var result = await _dataContext.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<Notification> GetNotificationByUserIdEventId(int userId, int eventId)
        {
            var result = await _dataContext.Notifications.Where(x=> x.UserId == userId &&  x.EventId == eventId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Notification> ToggleRead(Notification request)
        {
            request.IsRead = !request.IsRead;

            var result = _dataContext.Notifications.Update(request);
            await _dataContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<Notification>> DeleteAllFromEvent(int eventId)
        {
            var notifications = await _dataContext.Notifications.Where(x=>x.EventId == eventId).ToListAsync();
            foreach(var notification in notifications)
            {
                _dataContext.Notifications.Remove(notification);
            }
            await _dataContext.SaveChangesAsync();
            return notifications;
        }
    }
}
