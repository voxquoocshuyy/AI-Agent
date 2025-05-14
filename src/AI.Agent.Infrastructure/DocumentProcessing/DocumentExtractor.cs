using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Handles extraction of text content from various document formats
    /// </summary>
    public class DocumentExtractor : IDocumentExtractor
    {
        private readonly ILogger<DocumentExtractor> _logger;
        private readonly ITextExtractor _textExtractor;
        private readonly IPdfExtractor _pdfExtractor;
        private readonly ICsvExtractor _csvExtractor;

        public DocumentExtractor(
            ILogger<DocumentExtractor> logger,
            ITextExtractor textExtractor,
            IPdfExtractor pdfExtractor,
            ICsvExtractor csvExtractor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _textExtractor = textExtractor ?? throw new ArgumentNullException(nameof(textExtractor));
            _pdfExtractor = pdfExtractor ?? throw new ArgumentNullException(nameof(pdfExtractor));
            _csvExtractor = csvExtractor ?? throw new ArgumentNullException(nameof(csvExtractor));
        }

        /// <summary>
        /// Extracts text content from a document based on its file type
        /// </summary>
        /// <param name="fileStream">The document file stream</param>
        /// <param name="fileType">The type of document (e.g., pdf, txt, csv)</param>
        /// <returns>The extracted text content</returns>
        public async Task<string> ExtractTextAsync(Stream fileStream, string fileType)
        {
            try
            {
                _logger.LogInformation("Starting text extraction for file type: {FileType}", fileType);

                string content = fileType.ToLower() switch
                {
                    "pdf" => await _pdfExtractor.ExtractTextAsync(fileStream),
                    "txt" => await _textExtractor.ExtractTextAsync(fileStream),
                    "csv" => await _csvExtractor.ExtractTextAsync(fileStream),
                    _ => throw new NotSupportedException($"File type {fileType} is not supported")
                };

                _logger.LogInformation("Successfully extracted text from {FileType} document", fileType);
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from {FileType} document", fileType);
                throw;
            }
        }

        /// <summary>
        /// Validates if the file type is supported
        /// </summary>
        /// <param name="fileType">The file type to validate</param>
        /// <returns>True if the file type is supported, false otherwise</returns>
        public bool IsFileTypeSupported(string fileType)
        {
            return fileType.ToLower() switch
            {
                "pdf" or "txt" or "csv" => true,
                _ => false
            };
        }
    }
}