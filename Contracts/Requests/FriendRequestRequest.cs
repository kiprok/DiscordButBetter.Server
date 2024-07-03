namespace DiscordButBetter.Server.Contracts.Requests;

public enum RequestType
{
    Send,
    Accept,
    Decline,
    Cancel
}


public class FriendRequestRequest
{
    public RequestType Type { get; set; }
    public Guid UserId { get; set; }
    public Guid? RequestId { get; set; }
}