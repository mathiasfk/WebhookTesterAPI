using WebhookTester.API.SetupExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

const string corsPolicy = "sample-client";

builder.Services.AddCustomRateLimiting(builder.Configuration)
                .AddCustomDatabase(builder.Configuration)
                .AddCustomCors(builder.Configuration, corsPolicy)
                .AddCustomServices()
                .AddCustomSwagger()
                .AddAuthorization()
                .AddControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(app.Environment.IsProduction())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

await app.RunAsync();
