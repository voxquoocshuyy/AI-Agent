using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.HealthChecks;

public class AzureOpenAIHealthCheck : IHealthCheck
{
    private readonly IAzureOpenAIService _openAIService;
    private readonly ILogger<AzureOpenAIHealthCheck> _logger;

    public AzureOpenAIHealthCheck(
        IAzureOpenAIService openAIService,
        ILogger<AzureOpenAIHealthCheck> logger)
    {
        _openAIService = openAIService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isConnected = await _openAIService.TestConnectionAsync();
            return isConnected
                ? HealthCheckResult.Healthy("Azure OpenAI connection is healthy")
                : HealthCheckResult.Unhealthy("Azure OpenAI connection failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for Azure OpenAI");
            return HealthCheckResult.Unhealthy("Azure OpenAI health check failed", ex);
        }
    }
} 