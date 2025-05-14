using System.Globalization;
using System.Text;
using AI.Agent.Domain.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Implementation of ICsvExtractor for CSV files using CsvHelper
    /// </summary>
    public class CsvExtractor : ICsvExtractor
    {
        private readonly ILogger<CsvExtractor> _logger;

        public CsvExtractor(ILogger<CsvExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extracts text content from a CSV file
        /// </summary>
        /// <param name="fileStream">The CSV file stream</param>
        /// <returns>The extracted text content</returns>
        public async Task<string> ExtractTextAsync(Stream fileStream)
        {
            try
            {
                _logger.LogInformation("Starting text extraction from CSV file");

                var content = new StringBuilder();
                using var reader = new StreamReader(fileStream, Encoding.UTF8);
                using var csv = new CsvReader(reader,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true,
                        MissingFieldFound = null
                    });

                // Read header
                await csv.ReadAsync();
                csv.ReadHeader();
                content.AppendLine(string.Join(",", csv.HeaderRecord));

                // Read records
                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<dynamic>();
                    content.AppendLine(string.Join(",", ((IDictionary<string, object>)record).Values));
                }

                _logger.LogInformation("Successfully extracted text from CSV file");
                return content.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from CSV file");
                throw;
            }
        }
    }
}