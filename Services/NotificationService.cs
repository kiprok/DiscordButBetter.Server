using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Services;

public interface INotificationService
{
    Task<bool> SendFriendAddedNotification(Guid userId, Guid friendId);
}

public class NotificationService
    ( DbbContext db, IHubContext<NotificationHub, INotificationClient> hubContext)
    : INotificationService
{

    public Task<bool> SendFriendAddedNotification(Guid userId, Guid friendId)
    {
        throw new NotImplementedException();
    }
}