using System;
using AI.Agent.Core.Actions.Interfaces;

namespace AI.Agent.Core.Actions.Base;

/// <summary>
/// Base implementation of IActionResult
/// </summary>
public class ActionResult : IActionResult
{
    /// <inheritdoc />
    public bool Success { get; set; }

    /// <inheritdoc />
    public string Message { get; set; } = string.Empty;

    /// <inheritdoc />
    public object? Data { get; set; }

    /// <inheritdoc />
    public string? ErrorCode { get; set; }

    /// <inheritdoc />
    public object? ErrorDetails { get; set; }

    /// <inheritdoc />
    public long ExecutionTimeMs { get; set; }

    /// <inheritdoc />
    public DateTime CompletedAt { get; set; }

    /// <inheritdoc />
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="message">The success message</param>
    /// <param name="data">The result data</param>
    /// <returns>A successful action result</returns>
    public static ActionResult Success(string message, object? data = null)
    {
        return new ActionResult
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="errorDetails">The error details</param>
    /// <returns>A failed action result</returns>
    public static ActionResult Failure(string message, string errorCode, object? errorDetails = null)
    {
        return new ActionResult
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ErrorDetails = errorDetails
        };
    }
} 