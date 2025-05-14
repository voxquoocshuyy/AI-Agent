using System.Net;
using System.Text.Json;
using AI.Agent.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ApplicationException = AI.Agent.Infrastructure.Exceptions.ApplicationException;

namespace AI.Agent.Infrastructure.Middleware;

/// <summary>
/// Middleware for handling exceptions globally
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ApplicationException appException:
                response.StatusCode = (int)appException.StatusCode;
                errorResponse.ErrorCode = appException.ErrorCode;
                errorResponse.Message = appException.Message;

                if (appException is ValidationException validationException)
                {
                    errorResponse.Details = validationException.Errors;
                }
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.ErrorCode = "UNAUTHORIZED";
                errorResponse.Message = "You are not authorized to access this resource.";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred.";
                break;
        }

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

/// <summary>
/// Represents an error response
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error code
    /// </summary>
    public string ErrorCode { get; set; } = "UNKNOWN_ERROR";

    /// <summary>
    /// Gets or sets the error message
    /// </summary>
    public string Message { get; set; } = "An unexpected error occurred.";

    /// <summary>
    /// Gets or sets the trace identifier
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional error details
    /// </summary>
    public object? Details { get; set; }
} 