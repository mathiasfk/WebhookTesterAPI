using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Diagnostics.CodeAnalysis;

namespace WebhookTester.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class WebhookTesterDbContextFactory : IDesignTimeDbContextFactory<WebhookTesterDbContext>
    {
        public WebhookTesterDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebhookTesterDbContext>();
            optionsBuilder.UseSqlite("Data Source=webhooks.db");

            return new WebhookTesterDbContext(optionsBuilder.Options);
        }
    }
}
