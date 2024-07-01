namespace DiscordButBetter.Server.Contracts.Requests;

public class SendMessageRequest
{
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public string Metadata { get; set; }
}