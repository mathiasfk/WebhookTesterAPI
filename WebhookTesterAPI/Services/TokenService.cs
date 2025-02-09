namespace WebhookTesterAPI.Services
{
    public class TokenService
    {
        /// <summary>
        /// Generated a new random GUID to be used as authentication token.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <returns>A token</returns>
        public static IResult CreateToken(HttpContext context)
        {
            var token = Guid.NewGuid().ToString();
            return Results.Ok(new { token });
        }
    }
}
