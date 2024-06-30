using DiscordButBetter.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordButBetter.Server.Database;

public class DbbContext : DbContext
{
    public DbbContext(DbContextOptions<DbbContext> options) : base(options) { }
    
    public DbSet<UserModel> Users { get; set; }
    public DbSet<ConversationModel> Conversations { get; set; }
    public DbSet<ChatMessageModel> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.Friends)
            .WithMany()
            .UsingEntity("Friends",
                i => i.HasOne(typeof(UserModel)).WithMany().HasForeignKey("FriendId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("UserId"));
        
        modelBuilder.Entity<UserModel>()
            .HasMany(u => u.FriendRequests)
            .WithMany()
            .UsingEntity("FriendRequests",
                i => i.HasOne(typeof(UserModel)).WithMany().HasForeignKey("RequesterId"),
                j => j.HasOne(typeof(UserModel)).WithMany().HasForeignKey("RequestedId"));

    }
}