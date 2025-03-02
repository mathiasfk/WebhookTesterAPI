using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Entities;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.API.Controllers
{
    /// <summary>
    /// Handles requests to a webhook.
    /// </summary>
    /// <param name="service"></param>
    [ApiController]
    [Route("/")]
    public class RequestsController(IWebhookService service) : ControllerBase
    {
        /// <summary>
        /// Stores this request and broadcasts it to the subscribed clients.
        /// </summary>
        /// <param name="webhookId"></param>
        /// <returns>A success or error message.</returns>
        [HttpGet("{webhookId:guid}")]
        [HttpPost("{webhookId:guid}")]
        [HttpPut("{webhookId:guid}")]
        [HttpDelete("{webhookId:guid}")]
        [HttpPatch("{webhookId:guid}")]
        [HttpHead("{webhookId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HandleRequest(Guid webhookId)
        {
            var headers = GetHeaders();
            var request = new WebhookRequest
            {
                WebhookId = webhookId,
                HttpMethod = HttpContext.Request.Method,
                Headers = headers,
                Body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(),
                ReceivedAt = DateTimeOffset.UtcNow
            };

            var saved = await service.HandleRequestAsync(webhookId, request);
            return saved ? Ok(new { message = "Request saved" }) : NotFound();
        }

        private Dictionary<string, string?[]> GetHeaders()
        {
            return HttpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToArray());
        }
    }
}
