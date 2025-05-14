using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Azure.AI.OpenAI;
using System.Collections.Generic;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Generates vector embeddings for text content using Azure OpenAI
    /// </summary>
    public class EmbeddingGenerator : IEmbeddingGenerator
    {
        private readonly ILogger<EmbeddingGenerator> _logger;
        private readonly OpenAIClient _openAIClient;
        private readonly string _deploymentName;
        private readonly int _maxRetries;
        private readonly int _retryDelayMs;

        public EmbeddingGenerator(
            ILogger<EmbeddingGenerator> logger,
            OpenAIClient openAIClient,
            string deploymentName,
            int maxRetries = 3,
            int retryDelayMs = 1000)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
            _deploymentName = deploymentName ?? throw new ArgumentNullException(nameof(deploymentName));
            _maxRetries = maxRetries;
            _retryDelayMs = retryDelayMs;
        }

        /// <summary>
        /// Generates vector embeddings for text content
        /// </summary>
        /// <param name="text">The text content to generate embeddings for</param>
        /// <returns>The generated vector embeddings</returns>
        public async Task<float[]> GenerateEmbeddingsAsync(string text)
        {
            try
            {
                _logger.LogInformation("Starting embedding generation for text");

                var retryCount = 0;
                while (true)
                {
                    try
                    {
                        var embeddings = await _openAIClient.GetEmbeddingsAsync(
                            new EmbeddingsOptions(text)
                            {
                                DeploymentName = _deploymentName
                            });
                        
                        _logger.LogInformation("Successfully generated embeddings");
                        return embeddings.Value.Data[0].Embedding.ToArray();
                    }
                    catch (Exception ex) when (retryCount < _maxRetries)
                    {
                        retryCount++;
                        _logger.LogWarning(ex, "Error generating embeddings, retry {RetryCount} of {MaxRetries}",
                            retryCount, _maxRetries);
                        await Task.Delay(_retryDelayMs);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embeddings after {MaxRetries} retries", _maxRetries);
                throw;
            }
        }
    }
}