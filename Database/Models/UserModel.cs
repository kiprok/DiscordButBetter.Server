namespace DiscordButBetter.Server.Database.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte Status { get; set; }
    public bool Online { get; set; }
    public string ProfilePicture { get; set; }
    public string StatusMessage { get; set; }
    public string Biography { get; set; }
    
    public List<UserModel> Friends { get; set; }
    public List<UserModel> FriendRequests { get; set; }
    
    public List<ConversationModel> Conversations { get; set; }
    public List<ConversationModel> VisibleConversations { get; set; }
    public List<ChatMessageModel> ChatMessages { get; set; }
    
    public List<SessionModel> Sessions { get; set; }
}