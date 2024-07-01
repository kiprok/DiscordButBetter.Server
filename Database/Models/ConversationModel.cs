namespace DiscordButBetter.Server.Database.Models;

public class ConversationModel
{
    public Guid Id { get; set; }
    public string ConversationName { get; set; }
    public byte ConversationType { get; set; }
    public string ConversationPicture { get; set; }
    public List<UserModel> Participants { get; set; }
    public List<UserModel> ParticipantsVisible { get; set; }
    public List<ChatMessageModel> ChatMessages { get; set; }
}