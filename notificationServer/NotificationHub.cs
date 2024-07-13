using System.Collections.Concurrent;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.notificationServer;

public class NotificationHub(DbbContext db) : Hub<INotificationClient>
{
    public static ConcurrentDictionary<Guid,List<string>> ConnectedUsers = new();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"userId: {Context.User?.Claims.First().Value}");
        var userId = Guid.Parse(Context.User?.Claims.First().Value!);
        
        var user = await db.Users
            .Include(u => u.Conversations)
            .ThenInclude(c => c.Participants)
            .Include(u => u.Friends)
            .Include(u => u.ReceivedFriendRequests)
            .Include(u => u.SentFriendRequests)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            Context.Abort();
            return;
        }
        
        ConnectedUsers.AddOrUpdate(userId, new List<string> {Context.ConnectionId}, (_, list) =>
        {
            list.Add(Context.ConnectionId);
            return list;
        });

        foreach (var conversation in user.Conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            foreach (var participant in conversation.Participants)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, participant.Id.ToString());
            }
        }

        foreach (var friend in user.Friends)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, friend.Id.ToString());
        }

        foreach (var friendRequest in user.ReceivedFriendRequests)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, friendRequest.Id.ToString());
        }
        
        foreach (var friendRequest in user.SentFriendRequests)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, friendRequest.Id.ToString());
        }

        await Clients.Caller.InitializedUser();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.User?.Claims.First().Value!);
        ConnectedUsers.AddOrUpdate(userId, new List<string>(), (_, list) =>
        {
            list.Remove(Context.ConnectionId);
            return list;
        });
        return base.OnDisconnectedAsync(exception);
    }

    public static async Task AddToGroupAsync(IHubContext<NotificationHub, INotificationClient> hubContext, Guid userId, Guid groupId)
    {
        if (ConnectedUsers.TryGetValue(userId, out var connectionIds))
            foreach (var connectionId in connectionIds)
                await hubContext.Groups.AddToGroupAsync(connectionId, groupId.ToString());
    }
    
    public static async Task RemoveFromGroupAsync(IHubContext<NotificationHub, INotificationClient> hubContext, Guid userId, Guid groupId)
    {
        if (ConnectedUsers.TryGetValue(userId, out var connectionIds))
            foreach (var connectionId in connectionIds)
                await hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId.ToString());
    }
}