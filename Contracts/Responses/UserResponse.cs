namespace DiscordButBetter.Server.Contracts.Responses;

public class UserResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string ProfilePicture { get; set; }
    public string StatusMessage { get; set; }
    public string Biography { get; set; }
}