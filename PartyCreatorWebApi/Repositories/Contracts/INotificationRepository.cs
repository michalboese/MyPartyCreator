using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.Repositories.Contracts
{
    public interface INotificationRepository
    {
        Task<NotificationDto> CreateNotification(Notification request);
        Task<List<NotificationDto>> GetAllNotificationsOfUser(int id);
        Task<Notification> DeleteNotification(int id);
        Task<Notification> ToggleRead(Notification request);
        Task<Notification> GetNotification(int id);
        Task<List<NotificationDto>> CreateNotificationToAllGuests(Notification notification);
        Task<Notification> GetNotificationByUserIdEventId(int userId, int eventId);
        Task<List<Notification>> DeleteAllFromEvent(int eventId);
    }
}
