# OpenAPI and Swagger Configuration

## Overview
This project uses both the new .NET 9 OpenAPI features and Swashbuckle.AspNetCore for API documentation. This dual approach ensures compatibility and provides a rich API documentation experience.

## Configuration Requirements

### .NET 9 OpenAPI
- The project uses `Microsoft.AspNetCore.OpenApi` package (version 9.0.0-preview.1.24081.5 or later)
- Required methods:
  - `builder.Services.AddOpenApi()`
  - `app.MapOpenApi()`

### Swashbuckle.AspNetCore
- The project uses Swashbuckle.AspNetCore package (version 6.5.0 or later)
- Required methods:
  - `builder.Services.AddSwaggerServices()`
  - `app.UseSwaggerServices()`

## Important Notes
1. Both OpenAPI and Swagger configurations are required for full functionality
2. The OpenAPI configuration is part of .NET 9 and provides basic OpenAPI support
3. Swashbuckle.AspNetCore provides additional features like Swagger UI and enhanced documentation
4. Tests have been added to verify both configurations are properly set up

## Testing
The configuration is verified through unit tests in `SwaggerConfigurationTests.cs`:
- `AddSwaggerServices_RegistersRequiredServices`
- `AddSwaggerServices_ConfiguresSecurityDefinition`
- `UseSwaggerServices_ConfiguresSwaggerUI`
- `Program_ConfiguresBothOpenAPIAndSwagger`

## Future Considerations
- When .NET 9 is officially released, review if both configurations are still needed
- Consider consolidating to a single approach if one becomes the preferred method
- Monitor for any breaking changes in either OpenAPI or Swashbuckle.AspNetCore packages 