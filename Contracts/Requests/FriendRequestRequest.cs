namespace DiscordButBetter.Server.Contracts.Requests;

public enum ReqeustType
{
    Send,
    Accept,
    Decline,
    Cancel
}


public class FriendRequestRequest
{
    public ReqeustType Type { get; set; }
    public Guid UserId { get; set; }
    public Guid? RequestId { get; set; }
}