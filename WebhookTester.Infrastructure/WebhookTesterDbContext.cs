using Microsoft.EntityFrameworkCore;
using WebhookTester.Core.Entities;

namespace WebhookTester.Infrastructure
{
    public class WebhookTesterDbContext(DbContextOptions<WebhookTesterDbContext> options) : DbContext(options)
    {
        public DbSet<Webhook> Webhooks { get; set; } = null!;
        public DbSet<WebhookRequest> WebhookRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Webhook>()
                .HasMany(w => w.Requests)
                .WithOne(r => r.Webhook)
                .HasForeignKey(r => r.WebhookId);
        }
    }
}