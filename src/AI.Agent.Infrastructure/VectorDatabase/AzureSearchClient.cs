using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.VectorDatabase;

public class AzureSearchClient : IVectorStore
{
    private readonly AzureSearchConfiguration _configuration;
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _indexClient;
    private readonly ILogger<AzureSearchClient> _logger;

    public AzureSearchClient(
        AzureSearchConfiguration configuration,
        ILogger<AzureSearchClient> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var endpoint = new Uri(configuration.Endpoint);
        var credential = new Azure.AzureKeyCredential(configuration.ApiKey);
        _searchClient = new SearchClient(endpoint, configuration.IndexName, credential);
        _indexClient = new SearchIndexClient(endpoint, credential);
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

    public async Task<bool> AddDocumentAsync(SearchDocument document)
    {
        try
        {
            var response = await _searchClient.IndexDocumentsAsync(
                IndexDocumentsBatch.Upload(new[] { document }));
            return response.Value.Results[0].Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add document to search index");
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

    public async Task<IEnumerable<SearchResult>> SearchAsync(float[] queryVector, int topK = 5, string filter = null)
    {
        try
        {
            var searchOptions = new SearchOptions
            {
                Size = topK,
                Filter = filter,
                VectorSearch = new VectorSearchOptions
                {
                    Queries = { new VectorSearchQuery(queryVector, topK) }
                }
            };

            var response = await _searchClient.SearchAsync<SearchDocument>("*", searchOptions);
            return response.Value.GetResults().Select(r => new SearchResult
            {
                Id = r.Document.Id,
                Content = r.Document.Content,
                Score = r.Score ?? 0,
                Metadata = r.Document.Metadata
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search documents");
            return Enumerable.Empty<SearchResult>();
        }
    }

    public async Task<bool> DeleteDocumentAsync(string documentId)
    {
        try
        {
            var response = await _searchClient.DeleteDocumentsAsync(
                IndexDocumentsBatch.Delete(new[] { new SearchDocument { Id = documentId } }));
            return response.Value.Results[0].Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete document from search index");
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