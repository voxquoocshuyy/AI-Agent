using AI.Agent.Domain.Entities;
using AI.Agent.Infrastructure.VectorDatabase;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SearchDocument = AI.Agent.Infrastructure.VectorDatabase.SearchDocument;

namespace AI.Agent.UnitTests.VectorDatabase
{
    public class AzureSearchClientTests
    {
        private readonly Mock<ILogger<AzureSearchClient>> _loggerMock;
        private readonly Mock<SearchClient> _searchClientMock;
        private readonly Mock<SearchIndexClient> _searchIndexClientMock;
        private readonly AzureSearchClient _azureSearchClient;

        public AzureSearchClientTests()
        {
            _loggerMock = new Mock<ILogger<AzureSearchClient>>();
            _searchClientMock = new Mock<SearchClient>();
            _searchIndexClientMock = new Mock<SearchIndexClient>();
            _azureSearchClient = new AzureSearchClient(
                _loggerMock.Object,
                _searchClientMock.Object,
                _searchIndexClientMock.Object,
                maxRetries: 3,
                retryDelayMs: 100);
        }

        [Fact]
        public async Task AddDocumentAsync_NullDocument_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _azureSearchClient.AddDocumentAsync(null));
        }

        [Fact]
        public async Task AddDocumentAsync_ValidDocument_AddsSuccessfully()
        {
            // Arrange
            var document = new Document("test-id", "test.txt", "Test content")
            {
                FileType = "txt",
                Vector = new float[] { 0.1f, 0.2f, 0.3f },
                Metadata = new Dictionary<string, string> { { "key", "value" } }
            };

            // _searchClientMock
            //         .Setup(x => x.IndexDocumentsAsync(
            //             It.IsAny<IndexDocumentsBatch<Azure.Search.Documents.Models.SearchDocument>>(),
            //             It.IsAny<IndexDocumentsOptions>(),
            //             It.IsAny<CancellationToken>()))
            //         .ReturnsAsync(new IndexDocumentsResult(Enumerable.Empty<IndexingResult>()));

            // Act
            await _azureSearchClient.AddDocumentAsync(document);

            // Assert
            _searchClientMock.Verify(x => x.IndexDocumentsAsync(
                    It.IsAny<IndexDocumentsBatch<Azure.Search.Documents.Models.SearchDocument>>(),
                    It.IsAny<IndexDocumentsOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchAsync_NullVector_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _azureSearchClient.SearchAsync(null));
        }

        // [Fact]
        // public async Task SearchAsync_ValidVector_ReturnsResults()
        // {
        //     // Arrange
        //     var vector = new float[] { 0.1f, 0.2f, 0.3f };
        //     var searchResults = new List<SearchResult<SearchDocument>>
        //     {
        //         new SearchResult<SearchDocument>(
        //             new SearchDocument
        //             {
        //                 ["id"] = "test-id",
        //                 ["content"] = "Test content",
        //                 ["@search.score"] = 0.95
        //             },
        //             0.95)
        //     };
        //
        //     _searchClientMock
        //         .Setup(x => x.SearchAsync<SearchDocument>(
        //             It.IsAny<string>(),
        //             It.IsAny<SearchOptions>(),
        //             It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(Response.FromValue(
        //             new SearchResults<SearchDocument>(searchResults, null, null),
        //             null));
        //
        //     // Act
        //     var results = await _azureSearchClient.SearchAsync(vector);
        //
        //     // Assert
        //     Assert.Single(results);
        //     Assert.Equal("test-id", results[0].Id);
        //     Assert.Equal("Test content", results[0].Content);
        //     Assert.Equal(0.95, results[0].Score);
        // }

        [Fact]
        public async Task DeleteDocumentAsync_NullDocumentId_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _azureSearchClient.DeleteDocumentAsync(null));
        }

        // [Fact]
        // public async Task DeleteDocumentAsync_ValidDocumentId_DeletesSuccessfully()
        // {
        //     // Arrange
        //     var documentId = "test-id";
        //
        //     _searchClientMock
        //         .Setup(x => x.DeleteDocumentsAsync(
        //             It.IsAny<IndexDocumentsBatch<SearchDocument>>(),
        //             It.IsAny<IndexDocumentsOptions>(),
        //             It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(new IndexDocumentsResult(new List<IndexingResult>()));
        //
        //     // Act
        //     await _azureSearchClient.DeleteDocumentAsync(documentId);
        //
        //     // Assert
        //     _searchClientMock.Verify(x => x.DeleteDocumentsAsync(
        //             It.IsAny<IndexDocumentsBatch<SearchDocument>>(),
        //             It.IsAny<IndexDocumentsOptions>(),
        //             It.IsAny<CancellationToken>()),
        //         Times.Once);
        // }
        //
        // [Fact]
        // public async Task AddDocumentAsync_RetryOnFailure_SucceedsAfterRetry()
        // {
        //     // Arrange
        //     var document = new Document
        //     {
        //         Id = "test-id",
        //         Name = "test.txt",
        //         Content = "Test content",
        //         FileType = "txt",
        //         Vector = new float[] { 0.1f, 0.2f, 0.3f }
        //     };
        //
        //     var attemptCount = 0;
        //     _searchClientMock
        //         .Setup(x => x.IndexDocumentsAsync(
        //             It.IsAny<IndexDocumentsBatch<SearchDocument>>(),
        //             It.IsAny<IndexDocumentsOptions>(),
        //             It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(() =>
        //         {
        //             attemptCount++;
        //             if (attemptCount == 1)
        //             {
        //                 throw new Exception("Temporary failure");
        //             }
        //
        //             return new IndexDocumentsResult(new List<IndexingResult>());
        //         });
        //
        //     // Act
        //     await _azureSearchClient.AddDocumentAsync(document);
        //
        //     // Assert
        //     Assert.Equal(2, attemptCount);
        //     _searchClientMock.Verify(x => x.IndexDocumentsAsync(
        //             It.IsAny<IndexDocumentsBatch<SearchDocument>>(),
        //             It.IsAny<IndexDocumentsOptions>(),
        //             It.IsAny<CancellationToken>()),
        //         Times.Exactly(2));
        // }
    }
}