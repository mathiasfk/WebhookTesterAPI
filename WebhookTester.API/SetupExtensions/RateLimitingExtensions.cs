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
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        }));
            });

            return services;
        }
    }
}
