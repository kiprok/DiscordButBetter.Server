namespace DiscordButBetter.Server.Database.Models;

public class ConnectionModel
{
    public string Id { get; set; }
    public Guid UserId { get; set; }
    public UserModel User { get; set; }
    public Guid ServerId { get; set; }
    public ServerModel Server { get; set; }
}