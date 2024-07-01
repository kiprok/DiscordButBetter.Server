using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database.Models;

namespace DiscordButBetter.Server.Contracts.Mappers;

public static class MapChatMessageModel
{
    public static MessageResponse ToMessageResponse(this ChatMessageModel message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = message.Metadata
        };
    }
    
    public static ChatMessageModel ToChatMessageModel(this MessageResponse message)
    {
        return new ChatMessageModel
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = message.Metadata
        };
    }
    
    public static ChatMessageModel ToChatMessageModel(this SendMessageRequest request)
    {
        return new ChatMessageModel
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            Metadata = request.Metadata
        };
    }
}