using System.Threading.Tasks;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for generating vector embeddings from text content
    /// </summary>
    public interface IEmbeddingGenerator
    {
        /// <summary>
        /// Generates vector embeddings for text content
        /// </summary>
        /// <param name="text">The text content to generate embeddings for</param>
        /// <returns>The generated vector embeddings</returns>
        Task<float[]> GenerateEmbeddingsAsync(string text);
    }
} 