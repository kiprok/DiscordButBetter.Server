namespace DiscordButBetter.Server.Contracts.Messages;

public class ChangedConversationMessage
{
    public Guid ConversationId { get; set; }
    public Guid? OwnerId { get; set; }
    public string? ConversationName { get; set; }
    public string? ConversationPicture { get; set; }
    public List<Guid> Participants { get; set; }
    public List<Guid>? ParticipantsToAdd { get; set; }
    public List<Guid>? ParticipantsToRemove { get; set; }
}