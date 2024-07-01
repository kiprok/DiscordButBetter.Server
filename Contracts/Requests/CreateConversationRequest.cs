namespace DiscordButBetter.Server.Contracts.Requests;

public class CreateConversationRequest
{
    public string ConversationName { get; set; }
    public byte ConversationType { get; set; }
    public string? ConversationPicture { get; set; }
    public List<Guid> Participants { get; set; }
}