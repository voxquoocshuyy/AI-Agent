using System.Text;
using AI.Agent.Domain.Interfaces;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Implementation of IPdfExtractor for PDF files using iText7
    /// </summary>
    public class PdfExtractor : IPdfExtractor
    {
        private readonly ILogger<PdfExtractor> _logger;

        public PdfExtractor(ILogger<PdfExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extracts text content from a PDF file
        /// </summary>
        /// <param name="fileStream">The PDF file stream</param>
        /// <returns>The extracted text content</returns>
        public async Task<string> ExtractTextAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Starting text extraction from PDF file");

                var content = new StringBuilder();
                using var pdfReader = new PdfReader(fileStream);
                using var pdfDocument = new PdfDocument(pdfReader);

                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                {
                    var page = pdfDocument.GetPage(i);
                    var strategy = new LocationTextExtractionStrategy();
                    var currentText = PdfTextExtractor.GetTextFromPage(page, strategy);
                    content.AppendLine(currentText);
                }

                _logger.LogInformation("Successfully extracted text from PDF file");
                return content.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from PDF file");
                throw;
            }
        }
    }
}