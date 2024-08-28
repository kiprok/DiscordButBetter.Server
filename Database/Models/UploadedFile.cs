namespace DiscordButBetter.Server.Database.Models;

public class UploadedFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public Guid UploaderId { get; set; }
    public UserModel Uploader { get; set; }
    public string FileType { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Hash { get; set; }

}