using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebhookTester.Core.Entities;

namespace WebhookTester.Infrastructure
{
    public class WebhookTesterDbContext(DbContextOptions<WebhookTesterDbContext> options) : DbContext(options)
    {
        public DbSet<Token> Tokens { get; set; } = null!;
        public DbSet<Webhook> Webhooks { get; set; } = null!;
        public DbSet<WebhookRequest> WebhookRequests { get; set; } = null!;

        private readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Webhook>()
                .HasMany(w => w.Requests)
                .WithOne(r => r.Webhook)
                .HasForeignKey(r => r.WebhookId);

            modelBuilder.Entity<WebhookRequest>()
                .Property(r => r.Headers)
                .HasConversion(
                    h => JsonSerializer.Serialize(h, jsonOptions),
                    h=> JsonSerializer.Deserialize<Dictionary<string, string[]>>(h, jsonOptions) ?? new Dictionary<string, string[]>()
                );
        }
    }
}