namespace DiscordButBetter.Server.Contracts.Messages;

public class FriendRequestAcceptedMessage
{
    public Guid RequestId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}