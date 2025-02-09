using WebhookTesterAPI.Services;

namespace WebhookTesterAPI
{
    public static class Endpoints
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapPost("/token", TokenService.CreateToken)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Creates a user token",
                Description = "Creates a new user token that should be used as the Authorization header for other admnistrative requests.",
                Responses =
            {
                ["200"] = new() { Description = "Token created with success." }
            }
            });

            app.MapPost("/webhooks", async (WebhookService service, HttpContext context) => 
                await service.CreateWebhook(context))
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Creates a new webhook",
                Description = "Register a new webhook and returns its unique ID.",
                Responses =
                {
                    ["200"] = new() { Description = "Webhook created with success." },
                    ["401"] = new() { Description = "Absent or invalid token." }
                }
            });

            app.MapGet("/webhooks", async (WebhookService service, HttpContext context) => 
                await service.ListWebhooks(context))
            .WithOpenApi(operation => new(operation)
            {
                Summary = "List all webhooks",
                Description = "List all existing webhooks belonging to the authenticated user.",
                Responses =
                {
                    ["200"] = new() { Description = "Webhook list returned with success." },
                    ["401"] = new() { Description = "Absent or invalid token." }
                }
            });


            app.MapGet("/webhooks/{id:guid}/requests", async (WebhookService service, HttpContext context, Guid id) =>
                await service.GetWebhookRequests(context, id))
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get webhook requests",
                Description = "Retrieves all requests previously received by a specific webhook.",
                Responses =
                {
                    ["200"] = new() { Description = "Webhook list returned with success." },
                    ["401"] = new() { Description = "Absent or invalid token." },
                    ["404"] = new() { Description = "Webhook not found" }
                }
            });

            app.MapMethods("/{id:guid}", ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD"], async (WebhookService service, HttpContext context, Guid id) =>
            {
                return await service.SaveRequestAsync(context, id);
            });
        }
    }

}
