using DiscordButBetter.Server.Contracts.Messages;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database.Models;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapConversationModel
{
    public static ConversationResponse ToConversationResponse(this ConversationModel conversation)
    {
        return new ConversationResponse
        {
            ConversationId = conversation.Id,
            OwnerId = conversation.OwnerId,
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
    
    public static NewConversationMessage ToNewConversationMessage(this ConversationModel conversation)
    {
        return new NewConversationMessage
        {
            ConversationId = conversation.Id,
            OwnerId = conversation.OwnerId,
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
    
    public static AddedToConversationMessage ToAddedToConversationMessage(this ConversationModel conversation, Guid userId)
    {
        return new AddedToConversationMessage
        {
            UserId = userId,
            ConversationId = conversation.Id,
            OwnerId = conversation.OwnerId,
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
    
    public static RemovedFromConversationMessage ToRemovedFromConversationMessage(this ConversationModel conversation, Guid userId)
    {
        return new RemovedFromConversationMessage
        {
            UserId = userId,
            ConversationId = conversation.Id,
            OwnerId = conversation.OwnerId,
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
    
    public static ChangedConversationMessage ToChangedConversationMessage(this ConversationModel conversation)
    {
        return new ChangedConversationMessage
        {
            ConversationId = conversation.Id,
            OwnerId = conversation.OwnerId,
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
    
}