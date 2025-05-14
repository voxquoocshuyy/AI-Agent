using System.IO;
using System.Threading.Tasks;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for extracting text content from plain text files
    /// </summary>
    public interface ITextExtractor
    {
        /// <summary>
        /// Extracts text content from a plain text file
        /// </summary>
        /// <param name="fileStream">The text file stream</param>
        /// <returns>The extracted text content</returns>
        Task<string> ExtractTextAsync(Stream fileStream);
    }
} 