using System.ComponentModel.DataAnnotations;

namespace DiscordButBetter.Server.Database.Models;

public class SessionModel
{
    public Guid Id { get; set; }
    public Guid userId { get; set; }
    public UserModel user { get; set; }
    public string token { get; set; }
    public DateTime expiration { get; set; }
}