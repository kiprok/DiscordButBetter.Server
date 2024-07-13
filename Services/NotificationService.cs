using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Services;

public interface INotificationService
{
    // Messages
    Task SendMessageNotification(MessageResponse message);
    Task SendMessageEditedNotification(MessageResponse message);
    Task SendMessageDeletedNotification(MessageResponse message);
    
    // Friend Requests
    Task SendFriendRequestNotification(FriendRequestResponse request);
    Task SendFriendRequestAcceptedNotification(FriendRequestResponse request);
    Task SendFriendRequestDeclinedNotification(FriendRequestResponse request);
    Task SendFriendRequestCanceledNotification(FriendRequestResponse request);
    
    // Users
    Task SendFriendRemovedNotification(Guid userId, Guid friendId);
    
}

public class NotificationService
    ( DbbContext db, IHubContext<NotificationHub, INotificationClient> hubContext)
    : INotificationService
{

    // Messages
    public async Task SendMessageNotification(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).NewMessage(message);
    }

    public async Task SendMessageEditedNotification(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageEdited(message);
    }

    public async Task SendMessageDeletedNotification(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageDeleted(message.MessageId);
    }

    // Friend Requests
    public async Task SendFriendRequestNotification(FriendRequestResponse request)
    {
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequest(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequest(request);
    }
    
    public async Task SendFriendRequestAcceptedNotification(FriendRequestResponse request)
    {
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestAccepted(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestAccepted(request);
    }
    
    public async Task SendFriendRequestDeclinedNotification(FriendRequestResponse request)
    {
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestDeclined(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestDeclined(request);
    }
    
    public async Task SendFriendRequestCanceledNotification(FriendRequestResponse request)
    {
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestCanceled(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestCanceled(request);
    }
    
    // Users
    public async Task SendFriendRemovedNotification(Guid userId, Guid friendId)
    {
        await hubContext.Clients.User(userId.ToString()).FriendRemoved(friendId);
        await hubContext.Clients.User(friendId.ToString()).FriendRemoved(userId);
    }
}