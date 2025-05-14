using Microsoft.Extensions.Configuration;

namespace AI.Agent.Infrastructure.VectorDatabase;

public class AzureSearchConfiguration
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string IndexName { get; set; }
    public int VectorDimensions { get; set; } = 1536;
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public int BatchSize { get; set; } = 100;

    public static AzureSearchConfiguration FromConfiguration(IConfiguration configuration)
    {
        return new AzureSearchConfiguration
        {
            Endpoint = configuration["AzureSearch:Endpoint"],
            ApiKey = configuration["AzureSearch:ApiKey"],
            IndexName = configuration["AzureSearch:IndexName"],
            VectorDimensions = int.Parse(configuration["AzureSearch:VectorDimensions"] ?? "1536"),
            MaxRetries = int.Parse(configuration["AzureSearch:MaxRetries"] ?? "3"),
            RetryDelayMs = int.Parse(configuration["AzureSearch:RetryDelayMs"] ?? "1000"),
            BatchSize = int.Parse(configuration["AzureSearch:BatchSize"] ?? "100")
        };
    }
} 