﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using WebhookTester.API.ActionFilters;
using WebhookTester.Core.Common;
using WebhookTester.Core.Interfaces;
using static WebhookTester.API.Models.DataTransferObjects;

namespace WebhookTester.API.Controllers
{
    /// <summary>
    /// Handles webhook operations (listing, creating, deleting) and fetching requests from a specific webhook.
    /// </summary>
    /// <param name="webhookService"></param>
    /// <param name="configuration"></param>
    /// <param name="sseService"></param>
    [ApiController]
    [Route("[controller]")]
    [EnableRateLimiting("AuthenticatedRateLimit")]
    public class WebhooksController(
        IWebhookService webhookService,
        IConfiguration configuration,
        IServerSentEventsService sseService
        ) : ControllerBase
    {
        private string BaseUrl => configuration["BaseUrl"] ?? "http://localhost";
        private const string Token = "Token";

        /// <summary>
        /// Create a new webhook
        /// </summary>
        /// <returns>The created webhook info</returns>
        [HttpPost()]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        [ProducesResponseType(typeof(WebhookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post()
        {
            var token = (Guid)HttpContext.Items[Token]!;

            var result = await webhookService.CreateWebhook(token);
            var webhook = result.Data;

            var dto = new WebhookDto(webhook.Id, $"{BaseUrl}/{webhook.Id}");
            return Ok(dto);
        }

        /// <summary>
        /// List all webhooks for the authenticated user.
        /// </summary>
        /// <returns>A list of webhooks.</returns>
        [HttpGet()]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        [ProducesResponseType(typeof(WebhookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var token = (Guid)HttpContext.Items[Token]!;

            var result = await webhookService.ListWebhooks(token);
            var webhooks = result.Data;

            var dtos = webhooks.Select(w => new WebhookDto(w.Id, $"{BaseUrl}/{w.Id}"));
            return Ok(dtos);
        }

        /// <summary>
        /// Delete a webhook by ID.
        /// </summary>
        /// <param name="id">The ID of the webhook to delete.</param>
        /// <returns>A success or error message.</returns>
        [HttpDelete("{id:guid}")]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var token = (Guid)HttpContext.Items[Token]!;

            var result = await webhookService.DeleteWebhook(token, id);
            return result.Success ? Ok(new { message = "Webhook deleted" }) : NotFound();
        }

        /// <summary>
        /// Get all requests received by a specific webhook.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        /// <returns>A list of requests.</returns>
        [HttpGet("{id:guid}/requests")]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        [ProducesResponseType(typeof(WebhookRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRequests(Guid id)
        {
            var token = (Guid)HttpContext.Items[Token]!;

            var result = await webhookService.GetRequests(token, id);
            if (!result.Success && result.Error?.Code == ErrorCode.NotFound)
            {
                return NotFound();
            }
            var requests = result.Data;
            var dtos = requests.Select(r => new WebhookRequestDto(r.Id, r.HttpMethod, r.Headers, r.Body, r.ReceivedAt));

            return Ok(dtos);
        }

        /// <summary>
        /// Open a stream of requests received by a specific webhook.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        [HttpGet("{id:guid}/stream")]
        public async Task OpenRequestsStream(Guid id)
        {
            HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");

            var channel = sseService.GetOrCreateChannel(id);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await foreach (var request in channel.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                var dto = new WebhookRequestDto(request.Id, request.HttpMethod, request.Headers, request.Body, request.ReceivedAt);

                var json = JsonSerializer.Serialize(dto, options);
                await HttpContext.Response.WriteAsync($"data: {json}\n\n");
                await HttpContext.Response.Body.FlushAsync();
            }
        }
    }
}
