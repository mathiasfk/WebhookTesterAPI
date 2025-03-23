using WebhookTester.API.SetupExtensions;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicy = "sample-client";

builder.Services.AddCustomRateLimiting(builder.Configuration)
                .AddCustomDatabase(builder.Configuration)
                .AddCustomServices()
                .AddCustomSwagger()
                .AddCustomCors(corsPolicy)
                .AddAuthorization()
                .AddControllers();

var app = builder.Build();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthorization();
app.UseRateLimiter();

await app.RunAsync();
