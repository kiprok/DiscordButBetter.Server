using System.Text.Json.Nodes;
using DiscordButBetter.Server.Contracts.Responses;

namespace DiscordButBetter.Server.notificationServer;

public interface INotificationClient
{
    Task InitializedUser();
    
    // Messages
    Task NewMessage(MessageResponse message);
    Task MessageEdited(MessageResponse message);
    Task MessageDeleted(Guid message);
    
    // Friend Requests
    Task FriendRequest(FriendRequestResponse request);
    Task FriendRequestAccepted(FriendRequestResponse request);
    Task FriendRequestDeclined(FriendRequestResponse request);
    Task FriendRequestCanceled(FriendRequestResponse request);
    
    // Conversations
    Task CreatedNewConversation(ConversationResponse conversation);
    Task AddedToConversation(ConversationResponse conversation, Guid userId);
    Task RemovedFromConversation(Guid conversationId, Guid userId);
    Task ConversationInfoChanged(ConversationResponse conversation);
    
    // Users
    Task FriendRemoved(Guid friendId);
    Task UserInfoChanged(UserUpdateResponse user);
    
}