using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebhookTester.API.ActionFilters;
using WebhookTester.Core.Interfaces;
using WebhookTester.Core.Services;
using WebhookTester.Infrastructure;
using WebhookTester.Infrastructure.Repositories;
using WebhookTester.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Database setup
builder.Services.AddDbContext<WebhookTesterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency injection
builder.Services.AddControllers();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IWebhookService, WebhooksService>();
builder.Services.AddScoped<IWebhooksRepository, WebhooksRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddSingleton<IServerSentEventsService, ServerSentEventsService>();
builder.Services.AddScoped<ValidateTokenFilter>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped(typeof(ICache<>), typeof(MemoryCache<>));


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Webhook Tester API",
        Description = "API to create webhooks and monitor requests to them."
    });

    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Authorization token in the format: 'Authorization: {token}'"
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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
