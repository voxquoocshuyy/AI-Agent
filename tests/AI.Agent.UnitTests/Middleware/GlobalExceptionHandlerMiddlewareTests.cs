using System.Net;
using System.Text.Json;
using AI.Agent.Infrastructure.Exceptions;
using AI.Agent.Infrastructure.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.Agent.UnitTests.Middleware;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _environmentMock;
    private readonly RequestDelegate _next;
    private readonly GlobalExceptionHandlerMiddleware _middleware;
    private readonly HttpContext _context;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _environmentMock = new Mock<IWebHostEnvironment>();
        _next = new RequestDelegate(_ => throw new Exception("Test exception"));
        _middleware = new GlobalExceptionHandlerMiddleware(_next, _loggerMock.Object, _environmentMock.Object);
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionOccurs_ShouldReturnErrorResponse()
    {
        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _context.Response.ContentType.Should().Be("application/json");

        var responseBody = await GetResponseBody();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorCode.Should().Be("INTERNAL_SERVER_ERROR");
        errorResponse.Message.Should().Be("An unexpected error occurred.");
        errorResponse.TraceId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundExceptionOccurs_ShouldReturnNotFoundResponse()
    {
        // Arrange
        var notFoundException = new NotFoundException("Resource not found");
        var next = new RequestDelegate(_ => throw notFoundException);
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        var responseBody = await GetResponseBody();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorCode.Should().Be("NOT_FOUND");
        errorResponse.Message.Should().Be("Resource not found");
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldReturnBadRequestResponse()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "email", new[] { "Email is required" } }
        };
        var validationException = new ValidationException(errors);
        var next = new RequestDelegate(_ => throw validationException);
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object, _environmentMock.Object);

        // Act
        await middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var responseBody = await GetResponseBody();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.ErrorCode.Should().Be("VALIDATION_ERROR");
        errorResponse.Details.Should().NotBeNull();
    }

    private async Task<string> GetResponseBody()
    {
        _context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_context.Response.Body);
        return await reader.ReadToEndAsync();
    }
}