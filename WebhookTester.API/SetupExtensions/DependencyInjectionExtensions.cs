using WebhookTester.API.ActionFilters;
using WebhookTester.Core.Interfaces;
using WebhookTester.Core.Services;
using WebhookTester.Infrastructure.Repositories;
using WebhookTester.Infrastructure.Services;

namespace WebhookTester.API.SetupExtensions
{
    /// <summary>
    /// Extension methods for dependency injection setup.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds custom services to the application.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IWebhookService, WebhooksService>();
            services.AddScoped<IWebhooksRepository, WebhooksRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped(typeof(ICache<>), typeof(MemoryCache<>));
            services.AddScoped<ValidateTokenFilter>();

            services.AddSingleton<IServerSentEventsService, ServerSentEventsService>();

            services.AddMemoryCache();

            return services;
        }
    }
}


