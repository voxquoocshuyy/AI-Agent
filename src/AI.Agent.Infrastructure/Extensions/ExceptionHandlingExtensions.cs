using AI.Agent.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace AI.Agent.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring exception handling
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}