using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Agent.Infrastructure.ApiVersioning;

/// <summary>
/// Configuration for API versioning
/// </summary>
public static class ApiVersioningConfiguration
{
    /// <summary>
    /// Adds API versioning services to the application
    /// </summary>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("version")
            );
            options.ErrorResponses = new ApiVersioningErrorResponseProvider();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        return services;
    }
}

/// <summary>
/// Custom error response provider for API versioning
/// </summary>
public class ApiVersioningErrorResponseProvider : IErrorResponseProvider
{
    /// <summary>
    /// Creates an error response for API versioning errors
    /// </summary>
   public IActionResult CreateResponse(ErrorResponseContext context)
    {
        var response = new
        {
            Error = new
            {
                Code = "ApiVersionError",
                Message = context.Message,
                // SupportedVersions = context.ApiVersions?.Select(v => v.ToString()),
                // CurrentVersion = context.RequestedVersion?.ToString()
            }
        };

        return new BadRequestObjectResult(response);
    }
} 