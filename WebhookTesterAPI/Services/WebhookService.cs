using WebhookTesterAPI.DTOs;
using WebhookTesterAPI.Models;
using WebhookTesterAPI.Storage;

namespace WebhookTesterAPI
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
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            var webhook = new Webhook { OwnerToken = token };
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
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            var webhooks = await _repository.GetByTokenAsync(token);
            return Results.Ok(webhooks);
        }

        /// <summary>
        /// Returns all requests received by the specified webhook.
        /// </summary>
        /// <param name="context">The HTTP context containing request headers.</param>
        /// <param name="id">The ID of the webhook to get requests for.</param>
        /// <returns>A list of requests</returns>
        public async Task<IResult> GetWebhookRequests(HttpContext context, Guid id)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            var webhook = await _repository.GetByIdAsync(id);
            if (webhook == null)
                return Results.NotFound();

            var requests = webhook.Requests.Select(r => new WebhookRequestDTO(r.Id, r.HttpMethod, r.Headers, r.Body, r.ReceivedAt));

            return Results.Ok(requests);
        }

    }
}
