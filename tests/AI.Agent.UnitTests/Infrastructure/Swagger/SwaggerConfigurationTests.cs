using AI.Agent.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Xunit;

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

        var securityScheme = swaggerGenOptions?.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];
        Assert.NotNull(securityScheme);
        Assert.Equal("Bearer", securityScheme.Scheme);
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
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "swagger";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
            options.DocumentTitle = "Test API";
        });

        // Assert
        // Verify that both OpenAPI and Swagger endpoints are configured
        var endpointRouteBuilder = serviceProvider.GetRequiredService<IEndpointRouteBuilder>();
        var endpoints = endpointRouteBuilder.DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();
        Assert.NotNull(endpoints);
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
        // builder.Services.AddOpenApi(); // Ensure this method is defined or imported
        builder.Services.AddSwaggerServices();
        var app = builder.Build();
        // app.MapOpenApi(); // Ensure this method is defined or imported

        // Assert
        var endpoints = app.Services.GetRequiredService<IEndpointRouteBuilder>()
            .DataSources.SelectMany(ds => ds.Endpoints).ToList();
        Assert.Contains(endpoints, e => e.DisplayName?.Contains("swagger") == true);
        Assert.Contains(endpoints, e => e.DisplayName?.Contains("openapi") == true);
    }
}