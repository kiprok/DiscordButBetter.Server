using System.Text.Json.Nodes;

namespace DiscordButBetter.Server.Contracts.Messages;

public class EditChatMessageMessage
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public JsonObject Metadata { get; set; }
}