using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Database;

public class DbbContext : DbContext
{
    public DbbContext(DbContextOptions<DbbContext> options) : base(options)
    {
    }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<ConversationModel> Conversations { get; set; }
    public DbSet<ChatMessageModel> Messages { get; set; }
    public DbSet<SessionModel> Sessions { get; set; }
    public DbSet<FriendRequestModel> FriendRequests { get; set; }
    public DbSet<ServerModel> Servers { get; set; }
    public DbSet<ConnectionModel> Connections { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity("Friends",
                i => i.HasOne(typeof(UserModel)).WithMany().HasForeignKey("FriendId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("UserId"));

        modelBuilder.Entity<ConversationModel>()
            .HasOne(o => o.Owner)
            .WithMany(u => u.OwnedConversations)
            .HasForeignKey(c => c.OwnerId);
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.SentFriendRequests)
            .WithOne(r => r.Sender)
            .HasForeignKey(r => r.SenderId);

        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.ReceivedFriendRequests)
            .WithOne(r => r.Receiver)
            .HasForeignKey(r => r.ReceiverId);

        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.UploadedFiles)
            .WithOne(f => f.Uploader)
            .HasForeignKey(f => f.UploaderId);

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

        modelBuilder.Entity<ServerModel>()
            .HasMany(s => s.Connections)
            .WithOne(c => c.Server)
            .HasForeignKey(c => c.ServerId);

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Username);

        modelBuilder.Entity<ChatMessageModel>()
            .HasIndex(m => new { m.SentAt, m.Content });

        modelBuilder.Entity<UploadedFile>()
            .HasIndex(f => f.FileName);

        modelBuilder.Entity<UploadedFile>()
            .HasIndex(f => f.Hash);

        modelBuilder.Entity<ChatMessageModel>()
            .Property(m => m.Content)
            .HasColumnType("VARCHAR")
            .HasMaxLength(2000);

        modelBuilder.Entity<ConversationModel>()
            .Property(x => x.LastMessageTime)
            .HasConversion(d => d, d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

        modelBuilder.Entity<ChatMessageModel>()
            .Property(x => x.SentAt)
            .HasConversion(m => m, m => DateTime.SpecifyKind(m, DateTimeKind.Utc));

        modelBuilder.Entity<UserModel>()
            .Property(x => x.CreatedAt)
            .HasConversion(u => u, u => DateTime.SpecifyKind(u, DateTimeKind.Utc));

        modelBuilder.Entity<ServerModel>()
            .Property(x => x.LastPing)
            .HasConversion(p => p, p => DateTime.SpecifyKind(p, DateTimeKind.Utc));
        
        modelBuilder.Entity<ServerModel>()
            .Property(x => x.CreatedAt)
            .HasConversion(p => p, p => DateTime.SpecifyKind(p, DateTimeKind.Utc));
        
        modelBuilder.Entity<UploadedFile>()
            .Property(x => x.UploadedAt)
            .HasConversion(p => p, p => DateTime.SpecifyKind(p, DateTimeKind.Utc));
    }
}