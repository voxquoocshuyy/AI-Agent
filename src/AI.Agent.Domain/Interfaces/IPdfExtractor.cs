using System.IO;
using System.Threading.Tasks;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for extracting text content from PDF files
    /// </summary>
    public interface IPdfExtractor
    {
        /// <summary>
        /// Extracts text content from a PDF file
        /// </summary>
        /// <param name="fileStream">The PDF file stream</param>
        /// <returns>The extracted text content</returns>
        Task<string> ExtractTextAsync(Stream fileStream);
    }
} 