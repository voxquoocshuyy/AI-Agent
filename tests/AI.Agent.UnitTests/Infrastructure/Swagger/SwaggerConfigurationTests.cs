using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.OpenApi;

namespace AI.Agent.UnitTests.Infrastructure.Swagger;

public class SwaggerConfigurationTests
{
    [Fact]
    public void AddSwaggerServices_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<SwaggerGenOptions>();
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void AddSwaggerServices_ConfiguresSecurityDefinition()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSwaggerServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var swaggerGenOptions = serviceProvider.GetService<SwaggerGenOptions>();
        Assert.NotNull(swaggerGenOptions);

        var securityScheme = swaggerGenOptions?.SecurityDefinitions["Bearer"];
        Assert.NotNull(securityScheme);
        Assert.Equal("Bearer", securityScheme?.Scheme);
        Assert.Equal(SecuritySchemeType.ApiKey, securityScheme?.Type);
        Assert.Equal(ParameterLocation.Header, securityScheme?.In);
    }

    [Fact]
    public void UseSwaggerServices_ConfiguresSwaggerUI()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSwaggerServices();
        var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        app.UseSwaggerServices();

        // Assert
        // Verify that both OpenAPI and Swagger endpoints are configured
        var endpoints = app.Build().Endpoints;
        Assert.Contains(endpoints, e => e.DisplayName?.Contains("swagger") == true);
    }

    [Fact]
    public void Program_ConfiguresBothOpenAPIAndSwagger()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });

        // Act
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerServices();
        var app = builder.Build();
        app.MapOpenApi();

        // Assert
        var endpoints = app.Endpoints;
        Assert.Contains(endpoints, e => e.DisplayName?.Contains("swagger") == true);
        Assert.Contains(endpoints, e => e.DisplayName?.Contains("openapi") == true);
    }
} 