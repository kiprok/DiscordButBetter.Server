namespace DiscordButBetter.Server.Contracts.Messages;

public class FriendRequestDeclinedMessage
{
    public Guid RequestId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}