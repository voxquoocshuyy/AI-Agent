using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.Logging;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Request.Headers["X-Correlation-ID"] = correlationId;

        var sw = Stopwatch.StartNew();
        try
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["RequestPath"] = context.Request.Path,
                ["RequestMethod"] = context.Request.Method,
                ["UserId"] = context.User?.Identity?.Name ?? "anonymous"
            }))
            {
                _logger.LogInformation("Request started");
                await _next(context);
                sw.Stop();

                _logger.LogInformation("Request completed in {ElapsedMilliseconds}ms with status code {StatusCode}",
                    sw.ElapsedMilliseconds, context.Response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Request failed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
            throw;
        }
    }
} 