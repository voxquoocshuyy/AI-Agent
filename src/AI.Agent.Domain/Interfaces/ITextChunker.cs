using System.Collections.Generic;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for splitting text content into chunks
    /// </summary>
    public interface ITextChunker
    {
        /// <summary>
        /// Splits text content into chunks with overlap
        /// </summary>
        /// <param name="text">The text content to split</param>
        /// <returns>A list of text chunks</returns>
        IEnumerable<string> SplitIntoChunks(string text);
    }
} 