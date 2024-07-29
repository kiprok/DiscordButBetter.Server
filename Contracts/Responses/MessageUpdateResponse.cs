using System.Text.Json.Nodes;

namespace DiscordButBetter.Server.Contracts.Responses;

public class MessageUpdateResponse
{
    public Guid MessageId { get; set; }
    public string? Content { get; set; }
    public JsonObject? Metadata { get; set; }
    
}