using System;
using System.Collections.Generic;
using System.Threading;
using AI.Agent.Core.Actions.Interfaces;

namespace AI.Agent.Core.Actions.Base;

/// <summary>
/// Base implementation of IActionContext
/// </summary>
public class ActionContext : IActionContext
{
    /// <inheritdoc />
    public string ExecutionId { get; }

    /// <inheritdoc />
    public string UserId { get; }

    /// <inheritdoc />
    public string TenantId { get; }

    /// <inheritdoc />
    public string CorrelationId { get; }

    /// <inheritdoc />
    public IDictionary<string, object> Parameters { get; }

    /// <inheritdoc />
    public IDictionary<string, string> Metadata { get; }

    /// <inheritdoc />
    public CancellationToken CancellationToken { get; }

    /// <inheritdoc />
    public IActionContext? ParentContext { get; set; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; }

    /// <inheritdoc />
    public DateTime LastModifiedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ActionContext class
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="correlationId">The correlation identifier</param>
    /// <param name="parameters">The action parameters</param>
    /// <param name="metadata">The action metadata</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public ActionContext(
        string userId,
        string tenantId,
        string correlationId,
        IDictionary<string, object>? parameters = null,
        IDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ExecutionId = Guid.NewGuid().ToString();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        Parameters = parameters ?? new Dictionary<string, object>();
        Metadata = metadata ?? new Dictionary<string, string>();
        CancellationToken = cancellationToken;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = CreatedAt;
    }

    /// <summary>
    /// Updates the last modified timestamp
    /// </summary>
    public void UpdateLastModified()
    {
        LastModifiedAt = DateTime.UtcNow;
    }
} 