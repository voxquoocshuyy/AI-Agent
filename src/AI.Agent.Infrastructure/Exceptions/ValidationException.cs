using System.Net;

namespace AI.Agent.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : ApplicationException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public override string ErrorCode => "VALIDATION_ERROR";

    /// <summary>
    /// Gets the validation errors
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors) 
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string message, IDictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(string message, Exception innerException, IDictionary<string, string[]> errors) 
        : base(message, innerException)
    {
        Errors = errors;
    }
} 