using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Services;

public interface INotificationService
{
    Task SendFriendAddedNotification(Guid userId, Guid friendId);
    
    Task SendMessageNotification(MessageResponse message);
    
    Task SendFriendRequestNotification(FriendRequestResponse request);
}

public class NotificationService
    ( DbbContext db, IHubContext<NotificationHub, INotificationClient> hubContext)
    : INotificationService
{

    public Task SendFriendAddedNotification(Guid userId, Guid friendId)
    {
        throw new NotImplementedException();
    }

    public async Task SendMessageNotification(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).ReceiveMessage(message);
    }

    public async Task SendFriendRequestNotification(FriendRequestResponse request)
    {
        await hubContext.Clients.User(request.ReceiverId.ToString()).ReceiveFriendRequest(request);
    }
}