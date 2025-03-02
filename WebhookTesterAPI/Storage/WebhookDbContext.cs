﻿using Deprecated.WebhookTesterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Deprecated.WebhookTesterAPI.Storage
{
    public class WebhookDbContext(DbContextOptions<WebhookDbContext> options) : DbContext(options)
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