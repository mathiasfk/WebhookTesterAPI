namespace WebhookTesterAPI
{
    public class WebhookService
    {
        /// <summary>
        /// Creates a new webhook.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <returns>A JSON object with the webhook ID.</returns>
        public static IResult CreateWebhook(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            var webhookId = Guid.NewGuid().ToString();
            return Results.Ok(new { id = webhookId });
        }

        /// <summary>
        /// List existing webhooks of the authenticated user.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <returns>A list of all webhook IDs</returns>
        public static IResult ListWebhooks(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            return Results.Ok(new { webhooks = new List<string>() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult ListWebhookRequests(HttpContext context, string id)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            return Results.Ok(new { id, requests = new List<object>() });
        }

    }
}
