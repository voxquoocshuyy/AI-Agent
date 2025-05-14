using AI.Agent.Domain.Entities;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.VectorDatabase;

/// <summary>
/// Implementation of IVectorStore using Azure Cognitive Search
/// </summary>
public class AzureSearchClient : IVectorStore
{
    private readonly ILogger<AzureSearchClient> _logger;
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _indexClient;
    private readonly int _maxRetries;
    private readonly int _retryDelayMs;

    public AzureSearchClient(
        ILogger<AzureSearchClient> logger,
        SearchClient searchClient,
        SearchIndexClient indexClient,
        int maxRetries = 3,
        int retryDelayMs = 1000)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _searchClient = searchClient ?? throw new ArgumentNullException(nameof(searchClient));
        _indexClient = indexClient ?? throw new ArgumentNullException(nameof(indexClient));
        _maxRetries = maxRetries;
        _retryDelayMs = retryDelayMs;
    }

    /// <summary>
    /// Adds a document to the vector store
    /// </summary>
    /// <param name="document">The document to add</param>
    public async Task AddDocumentAsync(Document document)
    {
        try
        {
            _logger.LogInformation("Adding document {DocumentId} to vector store", document.Id);

            var retryCount = 0;
            while (true)
            {
                try
                {
                    var searchDocument = new SearchDocument
                    {
                        Id = document.Id,
                        Content = document.Content,
                        Vector = document.Vector,
                        Metadata = document.Metadata
                    };

                    await _searchClient.IndexDocumentsAsync(
                        IndexDocumentsBatch.Upload(new[] { searchDocument }));

                    _logger.LogInformation("Successfully added document {DocumentId} to vector store", document.Id);
                    return;
                }
                catch (Exception ex) when (retryCount < _maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning(ex, "Error adding document to vector store, retry {RetryCount} of {MaxRetries}",
                        retryCount, _maxRetries);
                    await Task.Delay(_retryDelayMs);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document to vector store after {MaxRetries} retries", _maxRetries);
            throw;
        }
    }

    /// <summary>
    /// Searches for similar documents using vector similarity
    /// </summary>
    /// <param name="vector">The query vector</param>
    /// <param name="limit">Maximum number of results to return</param>
    /// <returns>A list of similar documents</returns>
    public async Task<IEnumerable<SearchResult>> SearchAsync(float[] vector, int limit = 5)
    {
        try
        {
            _logger.LogInformation("Searching for similar documents");

            var retryCount = 0;
            while (true)
            {
                try
                {
                    var searchOptions = new SearchOptions
                    {
                        Size = limit,
                        Select = { "Id", "Content", "Metadata" },
                        VectorSearch = new VectorSearchOptions
                        {
                            Queries = { new VectorSearchQuery(vector) }
                        }
                    };

                    var results = await _searchClient.SearchAsync<SearchDocument>("*", searchOptions);
                    var searchResults = new List<SearchResult>();

                    await foreach (var result in results.Value.GetResultsAsync())
                    {
                        searchResults.Add(new SearchResult
                        {
                            Id = result.Document.Id,
                            Content = result.Document.Content,
                            Score = result.Score ?? 0,
                            Metadata = result.Document.Metadata
                        });
                    }

                    _logger.LogInformation("Found {ResultCount} similar documents", searchResults.Count);
                    return searchResults;
                }
                catch (Exception ex) when (retryCount < _maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning(ex, "Error searching vector store, retry {RetryCount} of {MaxRetries}",
                        retryCount, _maxRetries);
                    await Task.Delay(_retryDelayMs);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching vector store after {MaxRetries} retries", _maxRetries);
            throw;
        }
    }

    /// <summary>
    /// Deletes a document from the vector store
    /// </summary>
    /// <param name="documentId">The ID of the document to delete</param>
    public async Task DeleteDocumentAsync(string documentId)
    {
        try
        {
            _logger.LogInformation("Deleting document {DocumentId} from vector store", documentId);

            var retryCount = 0;
            while (true)
            {
                try
                {
                    await _searchClient.DeleteDocumentsAsync(
                        IndexDocumentsBatch.Delete(new[] { new SearchDocument { Id = documentId } }));

                    _logger.LogInformation("Successfully deleted document {DocumentId} from vector store", documentId);
                    return;
                }
                catch (Exception ex) when (retryCount < _maxRetries)
                {
                    retryCount++;
                    _logger.LogWarning(ex,
                        "Error deleting document from vector store, retry {RetryCount} of {MaxRetries}",
                        retryCount, _maxRetries);
                    await Task.Delay(_retryDelayMs);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document from vector store after {MaxRetries} retries", _maxRetries);
            throw;
        }
    }

    public async Task<bool> CreateIndexAsync()
    {
        try
        {
            var index = new SearchIndex(_configuration.IndexName)
            {
                Fields = new List<SearchField>
                {
                    new SearchField("id", SearchFieldDataType.String) { IsKey = true },
                    new SearchField("content", SearchFieldDataType.String) { IsSearchable = true },
                    new SearchField("vector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                    {
                        IsSearchable = true,
                        VectorSearchDimensions = _configuration.VectorDimensions,
                        VectorSearchConfiguration = "default"
                    },
                    new SearchField("metadata", SearchFieldDataType.String) { IsSearchable = true },
                    new SearchField("createdAt", SearchFieldDataType.DateTimeOffset) { IsFilterable = true },
                    new SearchField("updatedAt", SearchFieldDataType.DateTimeOffset) { IsFilterable = true }
                },
                VectorSearch = new VectorSearch
                {
                    AlgorithmConfigurations = new List<VectorSearchAlgorithmConfiguration>
                    {
                        new VectorSearchAlgorithmConfiguration("default", "hnsw")
                    }
                }
            };

            await _indexClient.CreateIndexAsync(index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create search index");
            return false;
        }
    }

    public async Task<bool> DeleteIndexAsync()
    {
        try
        {
            await _indexClient.DeleteIndexAsync(_configuration.IndexName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete search index");
            return false;
        }
    }

    public async Task<bool> AddDocumentsAsync(IEnumerable<SearchDocument> documents)
    {
        try
        {
            var batches = documents.Chunk(_configuration.BatchSize);
            foreach (var batch in batches)
            {
                var response = await _searchClient.IndexDocumentsAsync(
                    IndexDocumentsBatch.Upload(batch));
                if (!response.Value.Results.All(r => r.Succeeded))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add documents to search index");
            return false;
        }
    }

    public async Task<bool> DeleteDocumentsAsync(IEnumerable<string> documentIds)
    {
        try
        {
            var batches = documentIds.Chunk(_configuration.BatchSize);
            foreach (var batch in batches)
            {
                var response = await _searchClient.DeleteDocumentsAsync(
                    IndexDocumentsBatch.Delete(batch.Select(id => new SearchDocument { Id = id })));
                if (!response.Value.Results.All(r => r.Succeeded))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete documents from search index");
            return false;
        }
    }

    public async Task<bool> UpdateDocumentAsync(SearchDocument document)
    {
        try
        {
            var response = await _searchClient.IndexDocumentsAsync(
                IndexDocumentsBatch.Merge(new[] { document }));
            return response.Value.Results[0].Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update document in search index");
            return false;
        }
    }

    public async Task<bool> UpdateDocumentsAsync(IEnumerable<SearchDocument> documents)
    {
        try
        {
            var batches = documents.Chunk(_configuration.BatchSize);
            foreach (var batch in batches)
            {
                var response = await _searchClient.IndexDocumentsAsync(
                    IndexDocumentsBatch.Merge(batch));
                if (!response.Value.Results.All(r => r.Succeeded))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update documents in search index");
            return false;
        }
    }

    public async Task<SearchDocument> GetDocumentAsync(string documentId)
    {
        try
        {
            return await _searchClient.GetDocumentAsync<SearchDocument>(documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get document from search index");
            return null;
        }
    }
}