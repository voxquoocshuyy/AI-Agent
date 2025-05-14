using AI.Agent.Infrastructure.ApiVersioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Reflection;

namespace AI.Agent.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling API version deprecation warnings
/// </summary>
public class ApiVersionDeprecationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiVersionDeprecationMiddleware> _logger;

    public ApiVersionDeprecationMiddleware(
        RequestDelegate next,
        ILogger<ApiVersionDeprecationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var apiVersion = context.GetRequestedApiVersion();
        if (apiVersion == null)
        {
            await _next(context);
            return;
        }

        var deprecationAttribute = endpoint.Metadata
            .OfType<ApiVersionDeprecationAttribute>()
            .FirstOrDefault();

        if (deprecationAttribute != null)
        {
            var warningMessage = $"API version {apiVersion} is deprecated. " +
                               $"Deprecated on: {deprecationAttribute.DeprecationDate:yyyy-MM-dd}. " +
                               $"Will be removed on: {deprecationAttribute.SunsetDate:yyyy-MM-dd}. " +
                               deprecationAttribute.Message;

            _logger.LogWarning(warningMessage);
            context.Response.Headers.Add("Deprecation", "true");
            context.Response.Headers.Add("Sunset", deprecationAttribute.SunsetDate.ToString("R"));
            context.Response.Headers.Add("Link", $"<{context.Request.Path}>; rel=\"deprecation\"");
        }

        await _next(context);
    }
} 