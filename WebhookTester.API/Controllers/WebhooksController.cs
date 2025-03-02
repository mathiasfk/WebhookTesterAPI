using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Interfaces;
using static WebhookTester.API.Models.DataTransferObjects;

namespace WebhookTester.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhooksController(IWebhookService service, IConfiguration configuration) : ControllerBase
    {
        private string BaseUrl => configuration["BaseUrl"] ?? "http://localhost";

        [HttpPost()]
        [ProducesResponseType(typeof(WebhookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post()
        {
            var token = GetAndValidateToken();

            var webhook = await service.CreateWebhook(token);
            var dto = new WebhookDTO(webhook.Id, $"{BaseUrl}/{webhook.Id}");
            return Ok(dto);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(WebhookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var token = GetAndValidateToken();

            var webhooks = await service.ListWebhooks(token);
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

            var deleted = await service.DeleteWebhook(token, id);
            return deleted ? Ok(new { message = "Webhook deleted" }) : NotFound();
        }

        [HttpGet("{id:guid}/requests")]
        [ProducesResponseType(typeof(WebhookRequestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequests(Guid id)
        {
            var token = GetAndValidateToken();

            var requests = await service.GetRequests(token, id);
            if (!requests.Any())
            {
                // TODO: Return 404 only when the webhook is not found
                return NotFound();
            }
            var dtos = requests.Select(r => new WebhookRequestDTO(r.Id, r.HttpMethod, r.Headers, r.Body, r.ReceivedAt));

            return Ok(dtos);
        }

        [HttpGet("{id:guid}/stream")]
        public async Task<IActionResult> OpenRequestsStream(Guid id)
        {
            throw new NotImplementedException();
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
