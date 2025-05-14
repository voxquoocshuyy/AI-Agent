using Microsoft.AspNetCore.Mvc;

namespace AI.Agent.Infrastructure.ApiVersioning;

/// <summary>
/// Attribute to mark an API version as deprecated
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class ApiVersionDeprecationAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the deprecation date
    /// </summary>
    public DateTime DeprecationDate { get; }

    /// <summary>
    /// Gets or sets the sunset date
    /// </summary>
    public DateTime SunsetDate { get; }

    /// <summary>
    /// Gets or sets the deprecation message
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiVersionDeprecationAttribute"/> class
    /// </summary>
    /// <param name="deprecationDate">The date when the API version was deprecated</param>
    /// <param name="sunsetDate">The date when the API version will be removed</param>
    /// <param name="message">Optional message explaining the deprecation</param>
    public ApiVersionDeprecationAttribute(string deprecationDate, string sunsetDate, string message = "")
    {
        DeprecationDate = DateTime.Parse(deprecationDate);
        SunsetDate = DateTime.Parse(sunsetDate);
        Message = message;
    }
} 