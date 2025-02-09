using WebhookTesterAPI.DTOs;
using WebhookTesterAPI.Models;
using WebhookTesterAPI.Storage;

namespace WebhookTesterAPI.Services
{
    public class WebhookService(WebhookRepository repository)
    {
        private readonly WebhookRepository _repository = repository;

        /// <summary>
        /// Creates a new webhook.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <returns>A JSON object with the webhook ID.</returns>
        public async Task<IResult> CreateWebhook(HttpContext context)
        {
            var guidToken = GetTokenFromContext(context);
            if (guidToken == null)
                return Results.Unauthorized();

            var webhook = new Webhook { OwnerToken = guidToken.Value };
            await _repository.AddAsync(webhook);

            return Results.Ok(new { id = webhook.Id });
        }

        /// <summary>
        /// List existing webhooks of the authenticated user.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <returns>A list of all webhook IDs</returns>
        public async Task<IResult> ListWebhooks(HttpContext context)
        {
            var guidToken = GetTokenFromContext(context);
            if (guidToken == null)
                return Results.Unauthorized();

            var webhooks = await _repository.GetByTokenAsync(guidToken.Value);
            return Results.Ok(webhooks);
        }

        /// <summary>
        /// Delete a webhook and all requests associated with it.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <param name="id">The ID of the webhook.</param>
        /// <returns></returns>
        public async Task<IResult> DeleteWebhook(HttpContext context, Guid id)
        {
            var guidToken = GetTokenFromContext(context);
            if (guidToken == null)
                return Results.Unauthorized();

            var webhook = await _repository.GetByIdAsync(id);
            if (webhook == null || webhook.OwnerToken != guidToken.Value)
                return Results.NotFound();

            await _repository.RemoveAsync(webhook);

            return Results.Ok(new { message = "Webhook deleted" });
        }

        /// <summary>
        /// Returns all requests received by the specified webhook.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <param name="id">The ID of the webhook to get requests for.</param>
        /// <returns>A list of requests</returns>
        public async Task<IResult> GetWebhookRequests(HttpContext context, Guid id)
        {
            var guidToken = GetTokenFromContext(context);
            if (guidToken == null)
                return Results.Unauthorized();

            var webhook = await _repository.GetByIdAsync(id);
            if (webhook == null || webhook.OwnerToken != guidToken.Value)
                return Results.NotFound();

            var requests = webhook.Requests.Select(r => new WebhookRequestDTO(r.Id, r.HttpMethod, r.Headers, r.Body, r.ReceivedAt));

            return Results.Ok(requests);
        }

        /// <summary>
        /// Save a HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <param name="id">The ID of the webhook to get requests for.</param>
        /// <returns>An OK message</returns>
        public async Task<IResult> SaveRequestAsync(HttpContext context, Guid id)
        {
            var webhook = await _repository.GetByIdAsync(id);
            if (webhook == null)
                return Results.NotFound();

            var request = new WebhookRequest
            {
                WebhookId = webhook.Id,
                HttpMethod = context.Request.Method,
                Headers = string.Join("\n", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}")),
                Body = await new StreamReader(context.Request.Body).ReadToEndAsync(),
                ReceivedAt = DateTime.UtcNow
            };

            await _repository.AddRequestAsync(request);

            return Results.Ok(new { message = "Request saved", id = request.Id });
        }

        private static Guid? GetTokenFromContext(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token) || !Guid.TryParse(token, out Guid guidToken))
                return null;

            return guidToken;
        }

    }
}
