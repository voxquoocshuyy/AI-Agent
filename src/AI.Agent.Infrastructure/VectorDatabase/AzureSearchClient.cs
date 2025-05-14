using AI.Agent.Domain.Entities;
using AI.Agent.Domain.Interfaces;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

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
    private readonly AzureSearchConfiguration _configuration;
    private readonly string _indexName;

    public AzureSearchClient(
        ILogger<AzureSearchClient> logger,
        SearchClient searchClient,
        string indexName,
        int maxRetries = 3,
        int retryDelayMs = 1000)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _searchClient = searchClient ?? throw new ArgumentNullException(nameof(searchClient));
        _indexName = indexName ?? throw new ArgumentNullException(nameof(indexName));
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
            _logger.LogInformation("Adding document {DocumentId} to index {IndexName}", document.Id, _indexName);

            var searchDocument = new SearchDocument
            {
                Id = document.Id,
                Content = document.Content,
                Vector = document.Vector,
                Metadata = document.Metadata
            };

            await _searchClient.IndexDocumentsAsync(IndexDocumentsBatch.Upload(new[] { searchDocument }));
            _logger.LogInformation("Successfully added document {DocumentId}", document.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document {DocumentId} to index {IndexName}", document.Id, _indexName);
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
            _logger.LogInformation("Searching for similar documents in index {IndexName}", _indexName);

            var searchOptions = new SearchOptions
            {
                Size = limit,
                Select = { "id", "content", "vector", "metadata" }
            };

            var searchResults = await _searchClient.SearchAsync<SearchDocument>(
                "*",
                searchOptions);

            var results = new List<SearchResult>();
            await foreach (var result in searchResults.Value.GetResultsAsync())
            {
                results.Add(new SearchResult
                {
                    Id = result.Document.Id,
                    Content = result.Document.Content,
                    Score = (float)(result.Score ?? 0),
                    Metadata = result.Document.Metadata != null ? new Dictionary<string, string>(result.Document.Metadata) : new Dictionary<string, string>() as IReadOnlyDictionary<string, string>
                });
            }

            _logger.LogInformation("Found {Count} similar documents", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for similar documents in index {IndexName}", _indexName);
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
            _logger.LogInformation("Deleting document {DocumentId} from index {IndexName}", documentId, _indexName);

            await _searchClient.DeleteDocumentsAsync(new[] { new SearchDocument { Id = documentId } });
            _logger.LogInformation("Successfully deleted document {DocumentId}", documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId} from index {IndexName}", documentId, _indexName);
            throw;
        }
    }

    public async Task<bool> CreateIndexAsync()
    {
        try
        {
            _logger.LogInformation("Creating index {IndexName}", _indexName);

            var index = new SearchIndex(_indexName)
            {
                Fields = new List<SearchField>
                {
                    new SearchField("id", SearchFieldDataType.String)
                    {
                        IsKey = true,
                        IsSearchable = true,
                        IsFilterable = true
                    },
                    new SearchField("content", SearchFieldDataType.String)
                    {
                        IsSearchable = true,
                        IsFilterable = true
                    },
                    new SearchField("vector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                    {
                        IsSearchable = true,
                        IsFilterable = true,
                        VectorSearchDimensions = 1536,
                        VectorSearchConfiguration = "default-config"
                    },
                    new SearchField("metadata", SearchFieldDataType.Collection(SearchFieldDataType.String))
                    {
                        IsSearchable = true,
                        IsFilterable = true
                    }
                },
                VectorSearch = new VectorSearch
                {
                    AlgorithmConfigurations = new List<VectorSearchAlgorithmConfiguration>
                    {
                        new VectorSearchAlgorithmConfiguration("default-config")
                        {
                            Parameters = new HnswParameters
                            {
                                M = 4,
                                EfConstruction = 400,
                                EfSearch = 500,
                                Metric = VectorSearchAlgorithmMetric.Cosine
                            }
                        }
                    }
                }
            };

            await _indexClient.CreateOrUpdateIndexAsync(index);
            _logger.LogInformation("Successfully created index {IndexName}", _indexName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating index {IndexName}", _indexName);
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
                var response = await _searchClient.DeleteDocumentsAsync<SearchDocument>(
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

    public async Task<bool> DocumentExistsAsync(string documentId)
    {
        try
        {
            _logger.LogInformation("Checking if document {DocumentId} exists in index {IndexName}", documentId, _indexName);

            var searchOptions = new SearchOptions
            {
                Size = 1,
                Select = { "id" }
            };

            var searchResults = await _searchClient.SearchAsync<SearchDocument>(
                $"id eq '{documentId}'",
                searchOptions);

            var exists = await searchResults.Value.GetResultsAsync().FirstOrDefaultAsync() != null;
            _logger.LogInformation("Document {DocumentId} {Exists} in index {IndexName}",
                documentId, exists ? "exists" : "does not exist", _indexName);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if document {DocumentId} exists in index {IndexName}",
                documentId, _indexName);
            throw;
        }
    }
}

public class SearchDocument
{
    public string Id { get; set; }
    public string Content { get; set; }
    public float[] Vector { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
}