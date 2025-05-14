namespace AI.Agent.Core.Actions.Interfaces;

/// <summary>
/// Represents the result of executing an action
/// </summary>
public interface IActionResult
{
    /// <summary>
    /// Gets whether the action execution was successful
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets the message describing the result
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the data returned by the action
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets the error code if the action failed
    /// </summary>
    string? ErrorCode { get; }

    /// <summary>
    /// Gets the error details if the action failed
    /// </summary>
    object? ErrorDetails { get; }

    /// <summary>
    /// Gets the execution time in milliseconds
    /// </summary>
    long ExecutionTimeMs { get; }

    /// <summary>
    /// Gets the timestamp when the action completed
    /// </summary>
    DateTime CompletedAt { get; }

    /// <summary>
    /// Gets the correlation identifier for tracking the action
    /// </summary>
    string CorrelationId { get; }
} 