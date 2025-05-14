using AI.Agent.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AI.Agent.UnitTests.Infrastructure.Authentication;

public class JwtServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "your-256-bit-secret-key-here-for-testing-purposes",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationInMinutes = 60
        };

        _jwtService = new JwtService(Options.Create(_jwtSettings));
    }

    [Fact]
    public void GenerateToken_WithValidParameters_ReturnsValidToken()
    {
        // Arrange
        var userId = "test-user-id";
        var email = "test@example.com";
        var roles = new[] { "User", "Admin" };

        // Act
        var token = _jwtService.GenerateToken(userId, email, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var principal = _jwtService.ValidateToken(token);
        Assert.NotNull(principal);
        Assert.Equal(userId, principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal(email, principal.FindFirst(ClaimTypes.Email)?.Value);
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueTokens()
    {
        // Act
        var token1 = _jwtService.GenerateRefreshToken();
        var token2 = _jwtService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token1);
        Assert.NotNull(token2);
        Assert.NotEmpty(token1);
        Assert.NotEmpty(token2);
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var principal = _jwtService.ValidateToken(invalidToken);

        // Assert
        Assert.Null(principal);
    }
} 