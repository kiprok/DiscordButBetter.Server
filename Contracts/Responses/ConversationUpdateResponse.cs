namespace DiscordButBetter.Server.Contracts.Responses;

public class ConversationUpdateResponse
{
    public Guid ConversationId { get; set; }
    public string? ConversationName { get; set; }
    public string? ConversationPicture { get; set; }
    public List<Guid>? ParticipantsToAdd { get; set; }
    public List<Guid>? ParticipantsToRemove { get; set; }
}