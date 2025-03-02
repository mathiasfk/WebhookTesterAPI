using Deprecated.WebhookTesterAPI.DTOs;
using Deprecated.WebhookTesterAPI.Models;
using Deprecated.WebhookTesterAPI.Storage;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;

namespace Deprecated.WebhookTesterAPI.Services
{
    public class WebhookService(WebhookRepository repository, IConfiguration configuration)
    {
        private readonly WebhookRepository _repository = repository;
        private static readonly ConcurrentDictionary<Guid, Channel<WebhookRequestDTO>> _channels = new();
        private readonly IConfiguration _config = configuration;
        private string BaseUrl => _config["BaseUrl"] ?? "http://localhost";


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

            return Results.Ok(new WebhookDTO(webhook.Id, $"{BaseUrl}/{webhook.Id}"));
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
            var webhooksDTOs = webhooks.Select(w => new WebhookDTO(w.Id, $"{BaseUrl}/{w.Id}"));
            return Results.Ok(webhooksDTOs);
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

            var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray());
            var request = new WebhookRequest
            {
                WebhookId = webhook.Id,
                HttpMethod = context.Request.Method,
                Headers = headers,
                Body = await new StreamReader(context.Request.Body).ReadToEndAsync(),
                ReceivedAt = DateTimeOffset.UtcNow
            };

            await _repository.AddRequestAsync(request);

            var requestDto = new WebhookRequestDTO(request.Id, request.HttpMethod, request.Headers, request.Body, request.ReceivedAt);
            await NotifySubscribersAsync(id, requestDto);

            return Results.Ok(new { message = "Request saved", id = request.Id });
        }

        /// <summary>
        /// Opens a stream from all requests received by the specified webhook.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task StreamWebhookRequests(HttpContext context, Guid id)
        {
            context.Response.Headers.Append("Content-Type", "text/event-stream");

            var channel = _channels.GetOrAdd(id, _ => Channel.CreateUnbounded<WebhookRequestDTO>());
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await foreach (var request in channel.Reader.ReadAllAsync(context.RequestAborted))
            {
                var json = JsonSerializer.Serialize(request, options);
                await context.Response.WriteAsync($"data: {json}\n\n");
                await context.Response.Body.FlushAsync();
            }
        }

        private static Guid? GetTokenFromContext(HttpContext context)
        {
            var token = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token) || !Guid.TryParse(token, out Guid guidToken))
                return null;

            return guidToken;
        }

        private static async Task NotifySubscribersAsync(Guid id, WebhookRequestDTO request)
        {
            if (_channels.TryGetValue(id, out var channel))
            {
                await channel.Writer.WriteAsync(request);
            }
        }

    }
}