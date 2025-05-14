using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.HealthChecks;

public class AzureSearchHealthCheck : IHealthCheck
{
    private readonly IVectorStore _vectorStore;
    private readonly ILogger<AzureSearchHealthCheck> _logger;

    public AzureSearchHealthCheck(
        IVectorStore vectorStore,
        ILogger<AzureSearchHealthCheck> logger)
    {
        _vectorStore = vectorStore;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test search with empty vector
            var testVector = new float[1536];
            var results = await _vectorStore.SearchAsync(testVector, 1);

            return HealthCheckResult.Healthy("Azure Cognitive Search is healthy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for Azure Cognitive Search");
            return HealthCheckResult.Unhealthy("Azure Cognitive Search health check failed", ex);
        }
    }
}