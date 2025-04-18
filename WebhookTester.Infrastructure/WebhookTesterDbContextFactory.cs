﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace WebhookTester.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class WebhookTesterDbContextFactory : IDesignTimeDbContextFactory<WebhookTesterDbContext>
    {
        public WebhookTesterDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var appsettings = $"appsettings.{environment}.json";

            var basePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "WebhookTester.API");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(appsettings)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<WebhookTesterDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new WebhookTesterDbContext(optionsBuilder.Options);
        }
    }
}
