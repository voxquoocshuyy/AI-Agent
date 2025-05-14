using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Implementation of ITextExtractor for plain text files
    /// </summary>
    public class TextExtractor : ITextExtractor
    {
        private readonly ILogger<TextExtractor> _logger;

        public TextExtractor(ILogger<TextExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extracts text content from a plain text file
        /// </summary>
        /// <param name="fileStream">The text file stream</param>
        /// <returns>The extracted text content</returns>
        public async Task<string> ExtractTextAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Starting text extraction from plain text file");

                using var reader = new StreamReader(fileStream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();

                _logger.LogInformation("Successfully extracted text from plain text file");
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from plain text file");
                throw;
            }
        }
    }
} 