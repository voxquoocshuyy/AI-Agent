using System;
using System.Threading;
using System.Threading.Tasks;
using AI.Agent.Core.Actions.Interfaces;

namespace AI.Agent.Core.Actions.Base;

/// <summary>
/// Base class for implementing actions
/// </summary>
public abstract class BaseAction : IAction
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public virtual string Version => "1.0.0";

    /// <inheritdoc />
    public virtual string Category => "General";

    /// <inheritdoc />
    public virtual bool RequiresAuthentication => true;

    /// <inheritdoc />
    public virtual string[] RequiredPermissions => Array.Empty<string>();

    /// <inheritdoc />
    public async Task<IActionResult> ExecuteAsync(IActionContext context, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            if (!Validate(context))
            {
                return new ActionResult
                {
                    Success = false,
                    Message = "Action validation failed",
                    ErrorCode = "VALIDATION_ERROR",
                    ExecutionTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                    CompletedAt = DateTime.UtcNow,
                    CorrelationId = context.CorrelationId
                };
            }

            var result = await ExecuteInternalAsync(context, cancellationToken);
            result.ExecutionTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
            result.CompletedAt = DateTime.UtcNow;
            result.CorrelationId = context.CorrelationId;

            return result;
        }
        catch (Exception ex)
        {
            return new ActionResult
            {
                Success = false,
                Message = "An error occurred while executing the action",
                ErrorCode = "EXECUTION_ERROR",
                ErrorDetails = new { Exception = ex.Message, StackTrace = ex.StackTrace },
                ExecutionTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                CompletedAt = DateTime.UtcNow,
                CorrelationId = context.CorrelationId
            };
        }
    }

    /// <inheritdoc />
    public virtual bool Validate(IActionContext context)
    {
        if (context == null)
        {
            return false;
        }

        if (RequiresAuthentication && string.IsNullOrEmpty(context.UserId))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the action implementation
    /// </summary>
    /// <param name="context">The action context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The action result</returns>
    protected abstract Task<IActionResult> ExecuteInternalAsync(IActionContext context, CancellationToken cancellationToken);
} 