namespace WebhookTesterAPI
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this WebApplication app)
        {
            app.MapPost("/token", () =>
            {
                var token = Guid.NewGuid().ToString();
                return Results.Ok(new { token });
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Creates a user token",
                Description = "Creates a new user token that should be used as the Authorization header for other admnistrative requests.",
                Responses =
            {
                ["200"] = new() { Description = "Token created with success." }
            }
            });

            app.MapPost("/webhooks", (HttpContext context) =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token))
                    return Results.Unauthorized();

                var webhookId = Guid.NewGuid().ToString();
                return Results.Ok(new { id = webhookId });
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Creates a new webhook",
                Description = "Creates a new webhook and returns its unique ID.",
                Responses =
                {
                    ["200"] = new() { Description = "Webhook created with success." },
                    ["401"] = new() { Description = "Absent or invalid token." }
                }
            });

            app.MapGet("/webhooks", (HttpContext context) =>
            {
                var token = context.Request.Headers.Authorization.ToString();
                if (string.IsNullOrEmpty(token))
                    return Results.Unauthorized();

                return Results.Ok(new { webhooks = new List<string>() });
            })
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


            app.MapGet("/webhooks/{id}", (string id) =>
            {
                return Results.Ok(new { id, requests = new List<object>() });
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "List requests",
                Description = "List previously recorded requests for this specific webhook.",
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
