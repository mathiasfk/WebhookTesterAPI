using Microsoft.EntityFrameworkCore;
using WebhookTesterAPI;
using WebhookTesterAPI.Services;
using WebhookTesterAPI.Storage;

var builder = WebApplication.CreateBuilder(args);

// Database setup
builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseSqlite("Data Source=webhooks.db"));

// Dependency injection
builder.Services.AddScoped<WebhookRepository>();
builder.Services.AddScoped<WebhookService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Token de autenticação no formato: 'Authorization: {token}'"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
const string corsPolicy = "sample-client";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy,
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.MapEndpoints();
app.Run();
