namespace DiscordButBetter.Server.Contracts.Messages.Users;

public class FriendRemovedMessage
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
}