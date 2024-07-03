using System.Text.Json.Nodes;

namespace DiscordButBetter.Server.Contracts.Requests;

public class UpdateChatMessageRequest
{
    public string Content { get; set; }
    public JsonObject Metadata { get; set; }
}