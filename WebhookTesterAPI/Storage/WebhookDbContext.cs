using Microsoft.EntityFrameworkCore;
using WebhookTesterAPI.Models;

public class WebhookDbContext : DbContext
{
    public DbSet<Webhook> Webhooks { get; set; } = null!;
    public DbSet<WebhookRequest> WebhookRequests { get; set; } = null!;

    public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Webhook>()
            .HasMany(w => w.Requests)
            .WithOne(r => r.Webhook)
            .HasForeignKey(r => r.WebhookId);
    }
}
