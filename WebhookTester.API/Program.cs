using WebhookTester.API.SetupExtensions;

var builder = WebApplication.CreateBuilder(args);

const string corsPolicy = "sample-client";

builder.Services.AddCustomRateLimiting(builder.Configuration)
                .AddCustomDatabase(builder.Configuration)
                .AddCustomCors(builder.Configuration, corsPolicy)
                .AddCustomServices()
                .AddCustomSwagger()
                .AddAuthorization()
                .AddControllers();

var app = builder.Build();
app.MapControllers();

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

await app.RunAsync();
