using System.Threading;
using System.Threading.Tasks;

namespace AI.Agent.Core.Actions.Interfaces;

/// <summary>
/// Represents an action that can be executed by the AI Agent
/// </summary>
public interface IAction
{
    /// <summary>
    /// Gets the unique name of the action
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of what the action does
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the action
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the category of the action
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Gets whether the action requires authentication
    /// </summary>
    bool RequiresAuthentication { get; }

    /// <summary>
    /// Gets the required permissions for this action
    /// </summary>
    string[] RequiredPermissions { get; }

    /// <summary>
    /// Executes the action with the given context
    /// </summary>
    /// <param name="context">The context containing parameters and metadata for the action</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The result of the action execution</returns>
    Task<IActionResult> ExecuteAsync(IActionContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the action context before execution
    /// </summary>
    /// <param name="context">The context to validate</param>
    /// <returns>True if the context is valid, false otherwise</returns>
    bool Validate(IActionContext context);
} 