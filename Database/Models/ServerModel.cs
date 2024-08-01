namespace DiscordButBetter.Server.Database.Models;

public class ServerModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime LastPing { get; set; } = DateTime.UtcNow;
    public List<ConnectionModel> Connections { get; set; } = new();
}