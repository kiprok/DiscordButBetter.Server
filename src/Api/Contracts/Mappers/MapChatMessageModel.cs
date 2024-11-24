using System.Text.Json.Nodes;
using DiscordButBetter.Server.Contracts.Messages;
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
            MessageId = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = JsonNode.Parse(message.Metadata)?.AsObject() ?? new JsonObject() 
        };
    }
    
    public static ChatMessageModel ToChatMessageModel(this SendChatMessageRequest request)
    {
        return new ChatMessageModel
        {
            ConversationId = request.ConversationId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            Metadata = request.Metadata.ToString()
        };
    }
    
    public static SendChatMessageMessage ToSendChatMessageMessage(this ChatMessageModel message)
    {
        return new SendChatMessageMessage
        {
            MessageId = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = JsonNode.Parse(message.Metadata)?.AsObject() ?? new JsonObject()
        };
    }
    
    public static EditChatMessageMessage ToEditChatMessageMessage(this ChatMessageModel message)
    {
        return new EditChatMessageMessage
        {
            MessageId = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = JsonNode.Parse(message.Metadata)?.AsObject() ?? new JsonObject()
        };
    }
    
    public static DeleteChatMessageMessage ToDeleteChatMessageMessage(this ChatMessageModel message)
    {
        return new DeleteChatMessageMessage
        {
            MessageId = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt,
            Metadata = JsonNode.Parse(message.Metadata)?.AsObject() ?? new JsonObject()
        };
    }
    
}