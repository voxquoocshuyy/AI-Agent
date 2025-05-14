using Microsoft.Extensions.Configuration;

namespace AI.Agent.Infrastructure.AI;

public class AzureOpenAIConfiguration
{
    public string Endpoint { get; set; }
    public string ApiKey { get; set; }
    public string DeploymentName { get; set; }
    public string EmbeddingDeploymentName { get; set; }
    public int MaxRetries { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 1000;
    public int MaxTokens { get; set; } = 4000;
    public double Temperature { get; set; } = 0.7;
    public int RateLimitPerMinute { get; set; } = 60;

    public static AzureOpenAIConfiguration FromConfiguration(IConfiguration configuration)
    {
        return new AzureOpenAIConfiguration
        {
            Endpoint = configuration["AzureOpenAI:Endpoint"],
            ApiKey = configuration["AzureOpenAI:ApiKey"],
            DeploymentName = configuration["AzureOpenAI:DeploymentName"],
            EmbeddingDeploymentName = configuration["AzureOpenAI:EmbeddingDeploymentName"],
            MaxRetries = int.Parse(configuration["AzureOpenAI:MaxRetries"] ?? "3"),
            RetryDelayMs = int.Parse(configuration["AzureOpenAI:RetryDelayMs"] ?? "1000"),
            MaxTokens = int.Parse(configuration["AzureOpenAI:MaxTokens"] ?? "4000"),
            Temperature = double.Parse(configuration["AzureOpenAI:Temperature"] ?? "0.7"),
            RateLimitPerMinute = int.Parse(configuration["AzureOpenAI:RateLimitPerMinute"] ?? "60")
        };
    }
} 