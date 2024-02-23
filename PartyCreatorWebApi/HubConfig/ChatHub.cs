using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PartyCreatorWebApi.Dtos;
using PartyCreatorWebApi.Entities;

namespace PartyCreatorWebApi.HubConfig
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task SendChatMessage(ChatMessageDto message)
        {
            await Clients.Group(message.EventId.ToString()).SendAsync("ReceiveChatMessage", message);
        }

        public async Task SendNotification(string id, NotificationDto notification)
        {
            await Clients.User(id).SendAsync("ReceiveNotification", notification);
        }

        public async Task AddToEventGroup(string eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
            string message = $"{Context.UserIdentifier} has joined";
            //string user = $"{Context.UserIdentifier}";
            await Clients.Group(eventId).SendAsync("EventJoined", message);
        }

        public async Task RemoveFromEventGroup(string eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId);
            string message = $"{Context.UserIdentifier} has left";
            //string user = $"{Context.UserIdentifier}";
            await Clients.Group(eventId).SendAsync("EventLeft", message);
        }

    }
}
