using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Handles splitting text content into chunks for processing
    /// </summary>
    public class TextChunker : ITextChunker
    {
        private readonly ILogger<TextChunker> _logger;
        private readonly int _maxChunkSize;
        private readonly int _overlapSize;

        public TextChunker(
            ILogger<TextChunker> logger,
            int maxChunkSize = 1000,
            int overlapSize = 200)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maxChunkSize = maxChunkSize;
            _overlapSize = overlapSize;
        }

        /// <summary>
        /// Splits text content into chunks with overlap
        /// </summary>
        /// <param name="text">The text content to split</param>
        /// <returns>A list of text chunks</returns>
        public IEnumerable<string> SplitIntoChunks(string text)
        {
            try
            {
                _logger.LogInformation("Starting text chunking");

                if (string.IsNullOrWhiteSpace(text))
                {
                    return Enumerable.Empty<string>();
                }

                // Split text into sentences
                var sentences = Regex.Split(text, @"(?<=[.!?])\s+")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                var chunks = new List<string>();
                var currentChunk = new StringBuilder();

                foreach (var sentence in sentences)
                {
                    if (currentChunk.Length + sentence.Length > _maxChunkSize)
                    {
                        // Add current chunk to list
                        chunks.Add(currentChunk.ToString().Trim());
                        
                        // Start new chunk with overlap
                        var overlapText = GetOverlapText(currentChunk.ToString());
                        currentChunk.Clear();
                        currentChunk.Append(overlapText);
                    }

                    currentChunk.Append(sentence).Append(" ");
                }

                // Add the last chunk if it's not empty
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString().Trim());
                }

                _logger.LogInformation("Successfully split text into {ChunkCount} chunks", chunks.Count);
                return chunks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error splitting text into chunks");
                throw;
            }
        }

        private string GetOverlapText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= _overlapSize)
            {
                return text;
            }

            // Find the last sentence boundary within the overlap size
            var overlapText = text.Substring(text.Length - _overlapSize);
            var lastSentenceEnd = overlapText.LastIndexOfAny(new[] { '.', '!', '?' });

            if (lastSentenceEnd > 0)
            {
                return overlapText.Substring(lastSentenceEnd + 1).Trim();
            }

            return overlapText;
        }
    }
} 