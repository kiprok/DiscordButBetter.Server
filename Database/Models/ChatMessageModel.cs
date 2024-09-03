using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordButBetter.Server.Database.Models;

public class ChatMessageModel
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public ConversationModel Conversation { get; set; }
    public Guid SenderId { get; set; }
    public UserModel Sender { get; set; }
    [MaxLength(2000)]
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public string Metadata { get; set; }
}