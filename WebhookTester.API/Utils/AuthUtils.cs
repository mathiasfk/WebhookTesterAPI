namespace WebhookTester.API.Utils
{
    /// <summary>
    /// Utility methods for authentication.
    /// </summary>
    public static class AuthUtils
    {
        /// <summary>
        /// Get and validate the token from the request headers.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static Guid GetAndValidateToken(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token) || !Guid.TryParse(token, out Guid guidToken))
                throw new UnauthorizedAccessException();

            return guidToken;
        }
    }
}
