using System.ComponentModel.DataAnnotations;

namespace DiscordButBetter.Server.Database.Models;

public class ConnectionModel
{
    public string Id { get; set; }
    public Guid UserId { get; set; }
    public UserModel User { get; set; }
    [StringLength(40)] public string ServerId { get; set; }
    public ServerModel Server { get; set; }
}