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
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var publicRateLimitOptions = configuration.GetSection("RateLimiting:PublicRequests").Get<RateLimitOptions>();
            var authenticatedRateLimitOptions = configuration.GetSection("RateLimiting:AuthenticatedRequests").Get<RateLimitOptions>();

            if (publicRateLimitOptions == null || authenticatedRateLimitOptions == null)
            {
                throw new InvalidOperationException("Rate limit options are not configured properly.");
            }

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
                                PermitLimit = publicRateLimitOptions.PermitLimit,
                                QueueLimit = 0,
                                Window = publicRateLimitOptions.Window
                            });
                });

                options.AddPolicy("AuthenticatedRateLimit", context =>
                {
                    var authorization = context.Request.Headers.Authorization.ToString();
                    var partitionKey = string.IsNullOrEmpty(authorization) ? context.Request.Headers.Host.ToString() : authorization;

                    return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: partitionKey,
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = authenticatedRateLimitOptions.PermitLimit,
                                QueueLimit = 0,
                                Window = authenticatedRateLimitOptions.Window
                            });
                });
            });

            return services;
        }
    }

    /// <summary>
    /// Rate limit options.
    /// </summary>
    public class RateLimitOptions
    {
        /// <summary>
        /// The number of requests allowed in the specified window.
        /// </summary>
        public int PermitLimit { get; set; }
        /// <summary>
        /// The time window for the rate limit.
        /// </summary>
        public TimeSpan Window { get; set; }
    }
}
