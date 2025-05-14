using System.Collections.Generic;
using System.Threading;

namespace AI.Agent.Core.Actions.Interfaces;

/// <summary>
/// Represents the context for executing an action
/// </summary>
public interface IActionContext
{
    /// <summary>
    /// Gets the unique identifier of the action execution
    /// </summary>
    string ExecutionId { get; }

    /// <summary>
    /// Gets the user identifier who initiated the action
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Gets the tenant identifier
    /// </summary>
    string TenantId { get; }

    /// <summary>
    /// Gets the correlation identifier for tracking the action across services
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets the parameters for the action
    /// </summary>
    IDictionary<string, object> Parameters { get; }

    /// <summary>
    /// Gets the metadata for the action
    /// </summary>
    IDictionary<string, string> Metadata { get; }

    /// <summary>
    /// Gets the cancellation token for the action
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets the parent action context if this action is part of a chain
    /// </summary>
    IActionContext? ParentContext { get; set; }

    /// <summary>
    /// Gets the timestamp when the action was created
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the timestamp when the action was last modified
    /// </summary>
    DateTime LastModifiedAt { get; }
} 