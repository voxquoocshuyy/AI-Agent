# API Versioning

## Overview
The AI Agent API implements a comprehensive versioning strategy to ensure backward compatibility and smooth transitions between API versions. The versioning system supports multiple versioning schemes and includes deprecation management.

## Versioning Schemes

### URL Path Versioning
The primary versioning scheme uses URL paths:
```
/api/v1/weatherforecast
/api/v2/weatherforecast
```

### Header Versioning
Versions can also be specified using the `X-Version` header:
```
X-Version: 1.0
```

### Media Type Versioning
Versions can be specified in the Accept header:
```
Accept: application/json;version=1.0
```

## Version Deprecation

### Deprecation Attributes
API versions can be marked as deprecated using the `ApiVersionDeprecationAttribute`:
```csharp
[ApiVersionDeprecation(
    deprecationDate: "2024-03-20",
    sunsetDate: "2024-06-20",
    message: "This version will be removed in favor of v2.0")]
[ApiVersion("1.0")]
public class WeatherForecastController : ControllerBase
{
    // ...
}
```

### Deprecation Headers
When accessing a deprecated API version, the following headers are included in the response:
- `Deprecation: true`
- `Sunset: <date>`
- `Link: <url>; rel="deprecation"`

## Version Management

### Default Version
- Default version: v1.0
- Format: `v{major}.{minor}`
- Unspecified versions default to v1.0

### Version Selection
The API supports multiple version selection methods:
1. URL path (primary)
2. Header-based
3. Media type-based

## Error Handling

### Version Errors
When an invalid version is requested, a structured error response is returned:
```json
{
  "error": {
    "code": "ApiVersionError",
    "message": "The requested API version is not supported",
    "supportedVersions": ["1.0", "2.0"],
    "currentVersion": "3.0"
  }
}
```

## Best Practices

### Versioning Guidelines
1. Use semantic versioning (MAJOR.MINOR.PATCH)
2. Maintain backward compatibility within major versions
3. Document breaking changes
4. Provide migration guides
5. Set appropriate deprecation periods

### Deprecation Process
1. Mark version as deprecated
2. Set deprecation date
3. Set sunset date
4. Provide migration path
5. Monitor usage
6. Remove deprecated version

## Configuration

### API Versioning
```csharp
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("version")
    );
});
```

### Swagger Integration
The API versioning is integrated with Swagger UI:
- Multiple version documentation
- Version selection
- Deprecation warnings
- Version-specific examples

## Future Improvements
1. Add version-specific rate limiting
2. Implement version-specific caching
3. Add version analytics
4. Implement automatic version migration
5. Add version-specific monitoring
6. Implement version-specific security policies
7. Add version-specific documentation
8. Implement version-specific testing

## Development
To work with API versions:
1. Use version attributes
2. Test multiple versions
3. Monitor deprecation warnings
4. Update documentation
5. Test backward compatibility
6. Verify version headers
7. Test error responses
8. Validate Swagger documentation 