using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhooksController(IWebhookService webhookService) : ControllerBase
    {
        private readonly IWebhookService _service = webhookService;

        [HttpPost()]
        [ProducesResponseType(typeof(Webhook), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post()
        {
            var token = GetAndValidateToken();

            var webhook = await _service.CreateWebhook();
            return Ok(webhook);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(Webhook), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var token = GetAndValidateToken();

            var webhook = await _service.ListWebhooks();
            return Ok(webhook);
        }

        [HttpDelete("/{id:guid}")]
        [ProducesResponseType(typeof(Webhook), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = GetAndValidateToken();

            var deleted = await _service.DeleteWebhook(id);
            return deleted ? Ok(new { message = "Webhook deleted" }) : NotFound();
        }

        [HttpGet("/{id:guid}/requests")]
        [ProducesResponseType(typeof(Webhook), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequests(Guid id)
        {
            var token = GetAndValidateToken();

            var requests = await _service.GetRequests(id);
            if (!requests.Any())
            {
                // TODO: Return 404 only when the webhook is not found
                return NotFound();
            }

            return Ok(requests);
        }

        [HttpGet("/{id:guid}/stream")]
        [ProducesResponseType(typeof(Webhook), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
