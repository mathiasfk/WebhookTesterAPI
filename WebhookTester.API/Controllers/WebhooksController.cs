using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebhookTester.Core.Interfaces;
using static WebhookTester.API.Models.DataTransferObjects;

namespace WebhookTester.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhooksController(
        IWebhookService webhookService,
        IConfiguration configuration,
        IServerSentEventsService sseService) : ControllerBase
    {
        private string BaseUrl => configuration["BaseUrl"] ?? "http://localhost";

        [HttpPost()]
        [ProducesResponseType(typeof(WebhookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post()
        {
            var token = GetAndValidateToken();

            var webhook = await webhookService.CreateWebhook(token);
            var dto = new WebhookDTO(webhook.Id, $"{BaseUrl}/{webhook.Id}");
            return Ok(dto);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(WebhookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var token = GetAndValidateToken();

            var webhooks = await webhookService.ListWebhooks(token);
            var dtos = webhooks.Select(w => new WebhookDTO(w.Id, $"{BaseUrl}/{w.Id}"));
            return Ok(dtos);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = GetAndValidateToken();

            var deleted = await webhookService.DeleteWebhook(token, id);
            return deleted ? Ok(new { message = "Webhook deleted" }) : NotFound();
        }

        [HttpGet("{id:guid}/requests")]
        [ProducesResponseType(typeof(WebhookRequestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequests(Guid id)
        {
            var token = GetAndValidateToken();

            var requests = await webhookService.GetRequests(token, id);
            if (!requests.Any())
            {
                // TODO: Return 404 only when the webhook is not found
                return NotFound();
            }
            var dtos = requests.Select(r => new WebhookRequestDTO(r.Id, r.HttpMethod, r.Headers, r.Body, r.ReceivedAt));

            return Ok(dtos);
        }

        [HttpGet("{id:guid}/stream")]
        public async Task OpenRequestsStream(Guid id)
        {
            HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");

            var channel = sseService.GetOrCreateChannel(id);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await foreach (var request in channel.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                var dto = new WebhookRequestDTO(request.Id, request.HttpMethod, request.Headers, request.Body, request.ReceivedAt);

                var json = JsonSerializer.Serialize(dto, options);
                await HttpContext.Response.WriteAsync($"data: {json}\n\n");
                await HttpContext.Response.Body.FlushAsync();
            }
        }

        private Guid GetAndValidateToken()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(token) || !Guid.TryParse(token, out Guid guidToken))
                throw new UnauthorizedAccessException();

            return guidToken;
        }
    }
}
