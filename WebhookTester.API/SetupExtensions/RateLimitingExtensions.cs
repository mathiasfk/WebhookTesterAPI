using System.Threading.RateLimiting;

namespace WebhookTester.API.SetupExtensions
{
    /// <summary>
    /// Extension methods for rate limiting.
    /// </summary>
    public static class RateLimitingExtensions
    {
        /// <summary>
        /// Adds rate limiting to the application.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("PublicRequestsRateLimit", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: context.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 500,
                                QueueLimit = 0,
                                Window = TimeSpan.FromMinutes(1)
                            });
                });

                options.AddPolicy("AuthenticatedRateLimit", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: context.Request.Headers.Authorization.ToString() ?? context.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 50,
                                QueueLimit = 0,
                                Window = TimeSpan.FromMinutes(1)
                            });
                });
            });

            return services;
        }
    }
}
