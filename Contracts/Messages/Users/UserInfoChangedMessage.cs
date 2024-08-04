namespace DiscordButBetter.Server.Contracts.Messages.Users;

public class UserInfoChangedMessage
{
    public Guid UserId { get; set; }
    public string? ProfilePicture { get; set; }
    public byte? Status { get; set; }
    public string? StatusMessage { get; set; }
    public string? Biography { get; set; }
}