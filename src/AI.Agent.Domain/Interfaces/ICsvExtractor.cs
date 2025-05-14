using System.IO;
using System.Threading.Tasks;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for extracting text content from CSV files
    /// </summary>
    public interface ICsvExtractor
    {
        /// <summary>
        /// Extracts text content from a CSV file
        /// </summary>
        /// <param name="fileStream">The CSV file stream</param>
        /// <returns>The extracted text content</returns>
        Task<string> ExtractTextAsync(Stream fileStream);
    }
} 