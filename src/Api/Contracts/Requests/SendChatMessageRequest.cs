using System.Text.Json.Nodes;

namespace DiscordButBetter.Server.Contracts.Requests;

public class SendChatMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public JsonObject Metadata { get; set; }
}