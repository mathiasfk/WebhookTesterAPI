using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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

            modelBuilder.Entity<WebhookRequest>()
                .Property(r => r.Headers)
                .HasConversion(
                    h => JsonSerializer.Serialize(h, (JsonSerializerOptions)null),
                    h=> JsonSerializer.Deserialize<Dictionary<string, string[]>>(h, (JsonSerializerOptions)null) ?? new Dictionary<string, string[]>()
                );
        }
    }
}