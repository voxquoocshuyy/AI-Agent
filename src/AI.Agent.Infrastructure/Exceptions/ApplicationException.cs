using System.Net;

namespace AI.Agent.Infrastructure.Exceptions;

/// <summary>
/// Base exception class for all application-specific exceptions
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with this exception
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the error code associated with this exception
    /// </summary>
    public abstract string ErrorCode { get; }

    protected ApplicationException(string message) : base(message)
    {
    }

    protected ApplicationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
} 