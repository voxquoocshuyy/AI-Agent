using System.Collections.Generic;

namespace AI.Agent.Domain.Entities
{
    /// <summary>
    /// Represents a search result from the vector store
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Unique identifier of the document
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Content of the document
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Similarity score of the search result
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Additional metadata about the document
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
} 