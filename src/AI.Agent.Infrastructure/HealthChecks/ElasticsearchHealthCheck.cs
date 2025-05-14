using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AI.Agent.Infrastructure.HealthChecks;

public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _elasticsearchUrl;

    public ElasticsearchHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("ElasticsearchHealthCheck");
        _elasticsearchUrl = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_elasticsearchUrl}/_cluster/health", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("Elasticsearch is healthy");
            }

            return HealthCheckResult.Unhealthy("Elasticsearch is unhealthy",
                new Exception($"Status code: {response.StatusCode}"));
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Elasticsearch health check failed", ex);
        }
    }
}