using System.Collections.Generic;
using System.Threading.Tasks;
using AI.Agent.Domain.Entities;

namespace AI.Agent.Domain.Interfaces
{
    /// <summary>
    /// Interface for vector storage and search operations
    /// </summary>
    public interface IVectorStore
    {
        /// <summary>
        /// Adds a document to the vector store
        /// </summary>
        /// <param name="document">The document to add</param>
        Task AddDocumentAsync(Document document);

        /// <summary>
        /// Searches for similar documents using vector similarity
        /// </summary>
        /// <param name="vector">The query vector</param>
        /// <param name="limit">Maximum number of results to return</param>
        /// <returns>A list of similar documents</returns>
        Task<IEnumerable<SearchResult>> SearchAsync(float[] vector, int limit = 5);

        /// <summary>
        /// Deletes a document from the vector store
        /// </summary>
        /// <param name="documentId">The ID of the document to delete</param>
        Task DeleteDocumentAsync(string documentId);
    }
} 