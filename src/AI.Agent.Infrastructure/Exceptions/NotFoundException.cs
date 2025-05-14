using System.Net;

namespace AI.Agent.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : ApplicationException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public override string ErrorCode => "NOT_FOUND";

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public static NotFoundException Create<T>(string id)
    {
        return new NotFoundException($"Resource of type {typeof(T).Name} with id {id} was not found.");
    }
} 