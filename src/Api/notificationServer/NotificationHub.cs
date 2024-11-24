using System.Collections.Concurrent;
using DiscordButBetter.Server.Background;
using DiscordButBetter.Server.Contracts.Mappers;
using DiscordButBetter.Server.Contracts.Messages.Users;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using DiscordButBetter.Server.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.notificationServer;

public class NotificationHub(DbbContext db, IBus bus, ILogger<NotificationHub> logger) : Hub<INotificationClient>
{
    public static ConcurrentDictionary<Guid, ConcurrentList<string>> ConnectedUsers = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.User?.Claims.First().Value!);

        logger.LogInformation("User {UserId} connected with connectionId {connectionId}", userId, Context.ConnectionId);

        var user = await db.Users
            .AsSplitQuery()
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

        var connection = new ConnectionModel
        {
            Id = Context.ConnectionId,
            UserId = userId,
            ServerId = AppSettings.ServiceId
        };
        db.Connections.Add(connection);

        user.Online = true;
        await db.SaveChangesAsync();

        ConnectedUsers.AddOrUpdate(userId, new ConcurrentList<string>(Context.ConnectionId), (_, list) =>
        {
            list.Add(Context.ConnectionId);
            return list;
        });

        foreach (var conversation in user.Conversations)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
            foreach (var participant in conversation.Participants)
                await Groups.AddToGroupAsync(Context.ConnectionId, participant.Id.ToString());
        }

        foreach (var friend in user.Friends)
            await Groups.AddToGroupAsync(Context.ConnectionId, friend.Id.ToString());

        foreach (var friendRequest in user.ReceivedFriendRequests)
            await Groups.AddToGroupAsync(Context.ConnectionId, friendRequest.Id.ToString());

        foreach (var friendRequest in user.SentFriendRequests)
            await Groups.AddToGroupAsync(Context.ConnectionId, friendRequest.Id.ToString());

        await Clients.Caller.InitializedUser();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.User?.Claims.First().Value!);

        logger.LogInformation("User {UserId} disconnected with connectionId {connectionId}", userId,
            Context.ConnectionId);

        var connection = await db.Connections.FindAsync(Context.ConnectionId);
        if (connection is not null) db.Connections.Remove(connection);
        await db.SaveChangesAsync();

        var connections = db.Connections
            .Count(c => c.UserId == userId);

        if (connections == 0)
        {
            var user = await db.Users.FindAsync(userId);
            if (user is not null)
                if (user.Online)
                {
                    user.Online = false;
                    user.Status = 0;
                    var userUpdate = new UserInfoChangedMessage
                    {
                        UserId = userId,
                        Status = 0
                    };
                    //await Clients.Group(userId.ToString()).UserInfoChanged(userUpdate);
                    await bus.Publish(userUpdate);
                    await db.SaveChangesAsync();
                }
        }

        ConnectedUsers.AddOrUpdate(userId, new ConcurrentList<string>(), (_, list) =>
        {
            list.Remove(Context.ConnectionId);
            return list;
        });
    }

    public static async Task AddToGroupAsync(IHubContext<NotificationHub, INotificationClient> hubContext, Guid userId,
        Guid groupId)
    {
        if (ConnectedUsers.TryGetValue(userId, out var connectionIds))
            foreach (var connectionId in connectionIds.ToList())
                await hubContext.Groups.AddToGroupAsync(connectionId, groupId.ToString());
    }

    public static async Task RemoveFromGroupAsync(IHubContext<NotificationHub, INotificationClient> hubContext,
        Guid userId, Guid groupId)
    {
        if (ConnectedUsers.TryGetValue(userId, out var connectionIds))
            foreach (var connectionId in connectionIds.ToList())
                await hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId.ToString());
    }
}