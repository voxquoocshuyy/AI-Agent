using AI.Agent.Infrastructure.ApiVersioning;
using AI.Agent.Infrastructure.Authentication;
using AI.Agent.Infrastructure.HealthChecks;
using AI.Agent.Infrastructure.Logging;
using AI.Agent.Infrastructure.Middleware;
using AI.Agent.Infrastructure.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;
using AI.Agent.Infrastructure.Extensions;
using AI.Agent.Infrastructure.AzureOpenAI;
using AI.Agent.Infrastructure.AzureSearch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// Add logging
builder.Services.AddLoggingServices();

// Add performance optimization
builder.Services.AddPerformanceOptimization(builder.Configuration);

// Add authentication and authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Azure OpenAI services
var openAIConfig = AzureOpenAIConfiguration.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(openAIConfig);
builder.Services.AddSingleton<IAzureOpenAIService, AzureOpenAIService>();

// Add Azure Cognitive Search services
var searchConfig = AzureSearchConfiguration.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(searchConfig);
builder.Services.AddSingleton<IVectorStore, AzureSearchClient>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<ElasticsearchHealthCheck>("elasticsearch")
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."))
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'Redis' not found."))
    .AddCheck<AzureOpenAIHealthCheck>("azure-openai")
    .AddCheck<AzureSearchHealthCheck>("azure-search");

// Add API versioning
builder.Services.AddApiVersioningServices();

// Add Swagger services
builder.Services.AddSwaggerServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add security middleware
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseHttpsRedirection();
app.UseCors();

// Add performance monitoring middleware
app.UseMiddleware<PerformanceMonitoringMiddleware>();

// Add API version deprecation middleware
app.UseMiddleware<ApiVersionDeprecationMiddleware>();

// Configure authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new
            {
                Name = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description,
                Duration = x.Value.Duration
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", [Authorize(Policy = "RequireUserRole")] () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithApiVersionSet("v1")
.MapToApiVersion(new ApiVersion(1, 0));

// Add global exception handler
app.UseGlobalExceptionHandler();

// Add performance optimization middleware
app.UsePerformanceOptimization();

app.Run();

/// <summary>
/// Represents a weather forecast for a specific date.
/// </summary>
/// <param name="Date">The date of the forecast</param>
/// <param name="TemperatureC">The temperature in Celsius</param>
/// <param name="Summary">A summary of the weather conditions</param>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
