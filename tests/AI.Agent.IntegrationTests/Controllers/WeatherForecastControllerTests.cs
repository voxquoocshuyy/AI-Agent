using System.Net;
using FluentAssertions;
using Xunit;

namespace AI.Agent.IntegrationTests.Controllers;

public class WeatherForecastControllerTests : TestBase
{
    public WeatherForecastControllerTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Get_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/weatherforecast");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_WhenAuthenticated_ShouldReturnWeatherForecast()
    {
        // Arrange
        var token = await GetAuthToken();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Act
        var response = await Client.GetAsync("/weatherforecast");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();
    }

    private async Task<string> GetAuthToken()
    {
        // TODO: Implement actual token generation
        return "test-token";
    }
} 