using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Database;

public class DbbContext : DbContext
{
    public DbbContext(DbContextOptions<DbbContext> options) : base(options) { }
    
    public DbSet<UserModel> Users { get; set; }
    public DbSet<ConversationModel> Conversations { get; set; }
    public DbSet<ChatMessageModel> Messages { get; set; }
    public DbSet<SessionModel> Sessions { get; set; }
    public DbSet<FriendRequestModel> FriendRequests { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity("Friends",
                i => i.HasOne(typeof(UserModel)).WithMany().HasForeignKey("FriendId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("UserId"));
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.SentFriendRequests)
            .WithOne(r => r.Sender)
            .HasForeignKey(r => r.SenderId);
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.ReceivedFriendRequests)
            .WithOne(r => r.Receiver)
            .HasForeignKey(r => r.ReceiverId);
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Conversations)
            .WithMany(c => c.Participants)
            .UsingEntity("conversationParticipants",
                i => i.HasOne(typeof(ConversationModel)).WithMany().HasForeignKey("ConversationId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("UserId"));
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.VisibleConversations)
            .WithMany(c => c.ParticipantsVisible)
            .UsingEntity("conversationParticipantsVisible",
                i => i.HasOne(typeof(ConversationModel)).WithMany().HasForeignKey("ConversationId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("UserId"));

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Username);

        modelBuilder.Entity<ChatMessageModel>()
            .HasIndex(m => new {m.SentAt , m.Content});
        
        

    }
}