namespace DiscordButBetter.Server.Database.Models;

public class ConversationModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? OwnerId { get; set; } = null;
    public UserModel? Owner { get; set; } = null;
    public string ConversationName { get; set; }
    public byte ConversationType { get; set; }
    public string ConversationPicture { get; set; }

    public DateTime LastMessageTime { get; set; } = DateTime.UtcNow;
    public List<UserModel> Participants { get; set; }
    public List<UserModel> ParticipantsVisible { get; set; }
    public List<ChatMessageModel> ChatMessages { get; set; }
}