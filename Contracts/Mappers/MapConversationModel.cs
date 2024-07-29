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
            ConversationName = conversation.ConversationName,
            ConversationType = conversation.ConversationType,
            ConversationPicture = conversation.ConversationPicture,
            LastMessageTime = conversation.LastMessageTime,
            Participants = conversation.Participants.Select(c => c.Id).ToList()
        };
    }
}