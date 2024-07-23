namespace DiscordButBetter.Server.Contracts.Requests;

public class UploadFileRequest
{
    public string FileName { get; set; }
    public int FileSize { get; set; }
    public string FileType { get; set; }
}