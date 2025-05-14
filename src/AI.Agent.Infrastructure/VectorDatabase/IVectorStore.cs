namespace AI.Agent.Infrastructure.VectorDatabase;

public interface IVectorStore
{
    Task<bool> AddDocumentAsync(SearchDocument document);
    Task<bool> AddDocumentsAsync(IEnumerable<SearchDocument> documents);
    Task<IEnumerable<SearchResult>> SearchAsync(float[] queryVector, int topK = 5, string filter = null);
    Task<bool> DeleteDocumentAsync(string documentId);
    Task<bool> DeleteDocumentsAsync(IEnumerable<string> documentIds);
    Task<bool> UpdateDocumentAsync(SearchDocument document);
    Task<bool> UpdateDocumentsAsync(IEnumerable<SearchDocument> documents);
    Task<SearchDocument> GetDocumentAsync(string documentId);
    Task<bool> CreateIndexAsync();
    Task<bool> DeleteIndexAsync();
} 