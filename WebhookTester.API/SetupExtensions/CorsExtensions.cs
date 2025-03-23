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
        /// <param name="policyName"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomCors(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            return services;
        }
    }
}


