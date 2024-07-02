using System.ComponentModel.DataAnnotations;

namespace DiscordButBetter.Server.Database.Models;

public class FriendRequestModel
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public UserModel Sender { get; set; }
    public Guid ReceiverId { get; set; }
    public UserModel Receiver { get; set; }
}