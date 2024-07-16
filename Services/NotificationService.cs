using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.notificationServer;
using Microsoft.AspNetCore.SignalR;

namespace DiscordButBetter.Server.Services;

public interface INotificationService
{
    // Messages
    Task SendMessage(MessageResponse message);
    Task SendMessageEdited(MessageResponse message);
    Task SendMessageDeleted(MessageResponse message);
    
    // Friend Requests
    Task SendFriendRequest(FriendRequestResponse request);
    Task SendFriendRequestAccepted(FriendRequestResponse request);
    Task SendFriendRequestDeclined(FriendRequestResponse request);
    Task SendFriendRequestCanceled(FriendRequestResponse request);
    
    //conversations
    Task CreatedNewConversation(ConversationResponse conversation);
    Task AddedToConversation(ConversationResponse conversation, Guid userId);
    Task RemovedFromConversation(ConversationResponse conversation, Guid userId);
    Task ConversationInfoChanged(ConversationResponse conversation);
    
    // Users
    Task SendFriendRemoved(Guid userId, Guid friendId);
    Task UserInfoChanged(UserResponse user);
    
}

public class NotificationService
    ( DbbContext db, IHubContext<NotificationHub, INotificationClient> hubContext)
    : INotificationService
{

    // Messages
    public async Task SendMessage(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).NewMessage(message);
    }

    public async Task SendMessageEdited(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageEdited(message);
    }

    public async Task SendMessageDeleted(MessageResponse message)
    {
        await hubContext.Clients.Groups(message.ConversationId.ToString()).MessageDeleted(message.MessageId);
    }

    // Friend Requests
    public async Task SendFriendRequest(FriendRequestResponse request)
    {
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequest(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequest(request);
    }
    
    public async Task SendFriendRequestAccepted(FriendRequestResponse request)
    {
        await NotificationHub.AddToGroupAsync(hubContext, request.ReceiverId, request.SenderId);
        await NotificationHub.AddToGroupAsync(hubContext, request.SenderId, request.ReceiverId);
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestAccepted(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestAccepted(request);
    }
    
    public async Task SendFriendRequestDeclined(FriendRequestResponse request)
    {
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestDeclined(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestDeclined(request);
    }
    
    public async Task SendFriendRequestCanceled(FriendRequestResponse request)
    {
        await hubContext.Clients.User(request.ReceiverId.ToString()).FriendRequestCanceled(request);
        await hubContext.Clients.User(request.SenderId.ToString()).FriendRequestCanceled(request);
    }
    
    // Conversations
    public async Task CreatedNewConversation(ConversationResponse conversation)
    {
        foreach (var participant in conversation.Participants)
        {
            await NotificationHub.AddToGroupAsync(hubContext, participant, conversation.ConversationId);
            await hubContext.Clients.User(participant.ToString()).CreatedNewConversation(conversation);
        }
    }
    public async Task AddedToConversation(ConversationResponse conversation, Guid userId)
    {
        await NotificationHub.AddToGroupAsync(hubContext, userId, conversation.ConversationId);
        foreach (var participant in conversation.Participants)
        {
            await hubContext.Clients.User(participant.ToString()).AddedToConversation(conversation, userId);
        }
    }
    
    public async Task RemovedFromConversation(ConversationResponse conversation, Guid userId)
    {
        await NotificationHub.RemoveFromGroupAsync(hubContext, userId, conversation.ConversationId);
        await hubContext.Clients.User(userId.ToString()).RemovedFromConversation(conversation.ConversationId, userId);
        foreach (var participant in conversation.Participants)
        {
            await hubContext.Clients.User(participant.ToString()).RemovedFromConversation(conversation.ConversationId, userId);
        }
    }
    
    public async Task ConversationInfoChanged(ConversationResponse conversation)
    {
        foreach (var participant in conversation.Participants)
        {
            await hubContext.Clients.User(participant.ToString()).ConversationInfoChanged(conversation);
        }
    }
    
    
    // Users
    public async Task SendFriendRemoved(Guid userId, Guid friendId)
    {
        await hubContext.Clients.User(userId.ToString()).FriendRemoved(friendId);
        await hubContext.Clients.User(friendId.ToString()).FriendRemoved(userId);
    }
    
    public async Task UserInfoChanged(UserResponse user)
    {
        await hubContext.Clients.Group(user.UserId.ToString()).UserInfoChanged(user);
    }
    
}