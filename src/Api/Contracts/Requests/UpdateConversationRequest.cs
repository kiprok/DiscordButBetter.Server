namespace DiscordButBetter.Server.Contracts.Requests;

public class UpdateConversationRequest
{
    public string? ConversationName { get; set; }
    public string? ConversationPicture { get; set; }
    public List<Guid>? ParticipantsToAdd { get; set; }
    public List<Guid>? ParticipantsToRemove { get; set; }
    
}