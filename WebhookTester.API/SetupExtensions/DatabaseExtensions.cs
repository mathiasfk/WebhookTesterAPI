using Microsoft.EntityFrameworkCore;
using WebhookTester.Infrastructure;

namespace WebhookTester.API.SetupExtensions
{
    /// <summary>
    /// Extension methods for database setup.
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Adds database configuration to the application.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WebhookTesterDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}