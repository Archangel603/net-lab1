using Microsoft.EntityFrameworkCore;
using Server.Model;

namespace Server.Db;

public class ServerDbContext : DbContext
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ServerDbContext()
    {
        
    }
    
    public ServerDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5532;Database=lab1;Username=lab1;Password=1234");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasIndex(x => x.Username).IsUnique();
        });
        
        modelBuilder.Entity<Chat>(builder =>
        {
            builder.HasMany<User>(x => x.Users).WithMany(x => x.Chats);

            builder.Navigation(x => x.Users).AutoInclude();
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

            builder.Navigation(x => x.Sender).AutoInclude();
        });
    }
}