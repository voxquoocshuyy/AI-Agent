using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace AI.Agent.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    private readonly IApiVersionDescriptionProvider _versionProvider;

    public VersionController(IApiVersionDescriptionProvider versionProvider)
    {
        _versionProvider = versionProvider;
    }

    /// <summary>
    /// Gets the current API version information
    /// </summary>
    /// <returns>API version information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(VersionInfo), StatusCodes.Status200OK)]
    public IActionResult GetVersion()
    {
        var versions = _versionProvider.ApiVersionDescriptions
            .Select(description => new
            {
                Version = description.ApiVersion.ToString(),
                Status = description.IsDeprecated ? "Deprecated" : "Active",
                GroupName = description.GroupName
            })
            .ToList();

        var versionInfo = new VersionInfo
        {
            CurrentVersion = versions.First().Version,
            SupportedVersions = versions,
            ServerTime = DateTime.UtcNow
        };

        return Ok(versionInfo);
    }
}

/// <summary>
/// Represents API version information
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Gets or sets the current API version
    /// </summary>
    public string CurrentVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of supported API versions
    /// </summary>
    public IList<object> SupportedVersions { get; set; } = new List<object>();

    /// <summary>
    /// Gets or sets the server time in UTC
    /// </summary>
    public DateTime ServerTime { get; set; }
} 