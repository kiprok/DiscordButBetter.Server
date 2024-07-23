namespace DiscordButBetter.Server.Contracts.Responses;

public class UploadFileResponse
{
    public string UploadUrl { get; set; }
    public string NewFileName { get; set; }
    public string FileName { get; set; }
}