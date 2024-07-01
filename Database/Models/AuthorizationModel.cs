using System.ComponentModel.DataAnnotations;

namespace DiscordButBetter.Server.Database.Models;

public class AuthorizationModel
{
    [Key]
    public Guid userId { get; set; }
    public UserModel user { get; set; }
    public string token { get; set; }
    public DateTime expiration { get; set; }
}