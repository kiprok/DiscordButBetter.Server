using System.ComponentModel.DataAnnotations;

namespace DiscordButBetter.Server.Database.Models;

public class ServerModel
{
    [StringLength(40)] public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime LastPing { get; set; } = DateTime.UtcNow;
    public List<ConnectionModel> Connections { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}