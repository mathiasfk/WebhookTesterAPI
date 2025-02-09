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

            app.MapPost("/webhooks", WebhookService.CreateWebhook)
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

            app.MapGet("/webhooks", WebhookService.ListWebhooks)
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


            app.MapGet("/webhooks/{id}", WebhookService.ListWebhookRequests)
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get webhook requests",
                Description = "Retrieves all requests previously received by a specific webhook.",
                Responses =
            {
                ["200"] = new() { Description = "Webhook list returned with success." },
                ["401"] = new() { Description = "Absent or invalid token." }
            }
            });

            app.MapMethods("/{id}", ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD"], (HttpContext context, string id) =>
            {
                return Results.Ok(new { message = "Received", id });
            });
        }
    }

}
