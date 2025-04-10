namespace WebhookTester.API.SetupExtensions
{
    /// <summary>
    /// Extension methods for CORS setup.
    /// </summary>
    public static class CorsExtensions
    {
        /// <summary>
        /// Adds CORS to the application.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration, string policyName)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
            Console.WriteLine("Allowed Origins: " + string.Join(", ", allowedOrigins));


            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            return services;
        }
    }
}
