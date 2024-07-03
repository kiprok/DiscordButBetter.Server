namespace DiscordButBetter.Server.Contracts.Requests;

public class UpdateUserInfoRequest
{
    public string? Password { get; set; }
    public byte? Status { get; set; }
    public string? StatusMessage { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Biography { get; set; }
}