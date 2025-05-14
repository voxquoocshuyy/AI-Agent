using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.Middleware;

/// <summary>
/// Middleware for compressing HTTP responses
/// </summary>
public class ResponseCompressionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseCompressionMiddleware> _logger;

    public ResponseCompressionMiddleware(
        RequestDelegate next,
        ILogger<ResponseCompressionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();
        if (string.IsNullOrEmpty(acceptEncoding))
        {
            await _next(context);
            return;
        }

        if (acceptEncoding.Contains("gzip", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.Add("Content-Encoding", "gzip");
            context.Response.Body = new GZipStream(context.Response.Body, CompressionLevel.Fastest);
        }
        else if (acceptEncoding.Contains("deflate", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Headers.Add("Content-Encoding", "deflate");
            context.Response.Body = new DeflateStream(context.Response.Body, CompressionLevel.Fastest);
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error compressing response");
            throw;
        }
        finally
        {
            if (context.Response.Body is GZipStream gzip)
            {
                await gzip.DisposeAsync();
            }
            else if (context.Response.Body is DeflateStream deflate)
            {
                await deflate.DisposeAsync();
            }
        }
    }
} 