namespace DiscordButBetter.Server.Contracts.Messages;

public class RemovedFromConversationMessage
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public string ConversationName { get; set; }
    public byte ConversationType { get; set; }
    public string ConversationPicture { get; set; }
    public DateTime LastMessageTime { get; set; }
    public List<Guid>? Participants { get; set; }
}