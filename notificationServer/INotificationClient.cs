using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.Contracts.Messages.Users;

namespace DiscordButBetter.Server.notificationServer;

public interface INotificationClient
{
    Task InitializedUser();

    // Messages
    Task NewMessage(SendChatMessageMessage message);
    Task MessageEdited(EditChatMessageMessage message);
    Task MessageDeleted(Guid message);

    // Friend Requests
    Task FriendRequest(FriendRequestSendMessage request);
    Task FriendRequestAccepted(FriendRequestAcceptedMessage request);
    Task FriendRequestDeclined(FriendRequestDeclinedMessage request);
    Task FriendRequestCanceled(FriendRequestCanceledMessage request);

    // Conversations
    Task CreatedNewConversation(NewConversationMessage conversation);
    Task AddedToConversation(AddedToConversationMessage conversation, Guid userId);
    Task RemovedFromConversation(Guid conversationId, Guid userId);
    Task ConversationInfoChanged(ChangedConversationMessage conversation);

    // Users
    Task FriendRemoved(Guid friendId);
    Task UserInfoChanged(UserInfoChangedMessage user);
}