using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.notificationServer;

public class NotificationHub(DbbContext db) : Hub<INotificationClient>
{

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"userId: {Context.User?.Claims.First().Value}");
        var userId = Guid.Parse(Context.User?.Claims.First().Value!);
        
        var user = await db.Users.FindAsync(userId);

        if (user is null)
        {
            await Clients.Caller.ReceiveNotification("User not found", new UserResponse());
            return;
        }

        await Clients.All.ReceiveNotification($"A new user has connected: {Context.ConnectionId}", user.ToUserResponse());
    }
}