namespace DiscordButBetter.Server.Contracts.Responses;

public class FriendRequestResponse
{
    public Guid RequestId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}