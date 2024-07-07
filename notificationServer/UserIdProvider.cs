using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.notificationServer;

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Claims.First().Value;
    }
}