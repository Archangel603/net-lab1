using Microsoft.EntityFrameworkCore;
using Server.Model;

namespace Server.Db;

public class ServerDbContext : DbContext
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ServerDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Chat>(builder =>
        {
            builder.HasMany<User>(x => x.Users).WithMany(x => x.Chats);
        });
        
        modelBuilder.Entity<Message>(builder =>
        {
            builder.HasOne<User>(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.HasOne<Chat>(x => x.Chat)
                .WithMany()
                .HasForeignKey(x => x.ChatId);
        });
    }
}