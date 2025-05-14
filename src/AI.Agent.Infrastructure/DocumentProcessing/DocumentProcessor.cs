using System;
using System.IO;
using System.Threading.Tasks;
using AI.Agent.Domain.Entities;
using AI.Agent.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AI.Agent.Infrastructure.DocumentProcessing
{
    /// <summary>
    /// Handles the processing of documents, including text extraction, chunking, and vector generation
    /// </summary>
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly ILogger<DocumentProcessor> _logger;
        private readonly IDocumentExtractor _documentExtractor;
        private readonly ITextChunker _textChunker;
        private readonly IEmbeddingGenerator _embeddingGenerator;
        private readonly IVectorStore _vectorStore;

        public DocumentProcessor(
            ILogger<DocumentProcessor> logger,
            IDocumentExtractor documentExtractor,
            ITextChunker textChunker,
            IEmbeddingGenerator embeddingGenerator,
            IVectorStore vectorStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _documentExtractor = documentExtractor ?? throw new ArgumentNullException(nameof(documentExtractor));
            _textChunker = textChunker ?? throw new ArgumentNullException(nameof(textChunker));
            _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
            _vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        }

        /// <summary>
        /// Processes a document by extracting text, chunking, and generating vectors
        /// </summary>
        /// <param name="fileStream">The document file stream</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="fileType">The type of the file</param>
        /// <returns>The processed document</returns>
        public async Task<Document> ProcessAsync(Stream fileStream, string fileName, string fileType)
        {
            try
            {
                _logger.LogInformation("Starting document processing for {FileName}", fileName);

                // Extract text from document
                var content = await _documentExtractor.ExtractTextAsync(fileStream, fileType);

                // Create document
                var document = new Document(fileName, content, fileType)
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "OriginalFileName", fileName },
                        { "FileType", fileType },
                        { "ProcessedAt", DateTime.UtcNow.ToString("o") }
                    }
                };

                // Split content into chunks
                var chunks = _textChunker.SplitIntoChunks(content);
                var chunkIndex = 0;
                var totalChunks = chunks.Count();

                foreach (var chunk in chunks)
                {
                    // Generate embeddings for chunk
                    var vector = await _embeddingGenerator.GenerateEmbeddingsAsync(chunk);

                    // Create chunk document
                    var chunkDocument = new Document($"{fileName}_chunk_{chunkIndex}", chunk, fileType)
                    {
                        Id = $"{document.Id}_chunk_{chunkIndex}",
                        Vector = vector,
                        CreatedAt = DateTime.UtcNow,
                        IsProcessed = true,
                        Metadata = new Dictionary<string, string>
                        {
                            { "OriginalDocumentId", document.Id },
                            { "ChunkIndex", chunkIndex.ToString() },
                            { "TotalChunks", totalChunks.ToString() }
                        }
                    };

                    // Store chunk in vector store
                    await _vectorStore.AddDocumentAsync(chunkDocument);
                    chunkIndex++;
                }

                document.IsProcessed = true;
                _logger.LogInformation("Successfully processed document {FileName}", fileName);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing document {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Chunks a document into smaller pieces
        /// </summary>
        /// <param name="document">The document to chunk</param>
        /// <returns>A list of chunked documents</returns>
        public async Task<IEnumerable<Document>> ChunkAsync(Document document)
        {
            try
            {
                _logger.LogInformation("Starting document chunking for {DocumentId}", document.Id);

                var chunks = _textChunker.SplitIntoChunks(document.Content);
                var chunkedDocuments = new List<Document>();
                var chunkIndex = 0;

                foreach (var chunk in chunks)
                {
                    // Generate embeddings for chunk
                    var vector = await _embeddingGenerator.GenerateEmbeddingsAsync(chunk);

                    // Create chunk document
                    var chunkDocument = new Document($"{document.Name}_chunk_{chunkIndex}", chunk, document.FileType)
                    {
                        Id = $"{document.Id}_chunk_{chunkIndex}",
                        Vector = vector,
                        CreatedAt = DateTime.UtcNow,
                        IsProcessed = true,
                        Metadata = new Dictionary<string, string>
                        {
                            { "OriginalDocumentId", document.Id },
                            { "ChunkIndex", chunkIndex.ToString() },
                            { "TotalChunks", chunks.Count.ToString() }
                        }
                    };

                    chunkedDocuments.Add(chunkDocument);
                    chunkIndex++;
                }

                _logger.LogInformation("Successfully chunked document {DocumentId} into {ChunkCount} chunks", 
                    document.Id, chunkedDocuments.Count);
                return chunkedDocuments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error chunking document {DocumentId}", document.Id);
                throw;
            }
        }

        private async Task<IEnumerable<Document>> ProcessChunksAsync(IEnumerable<string> chunks, string sourceId)
        {
            var documents = new List<Document>();
            var chunkIndex = 0;

            foreach (var chunk in chunks)
            {
                var embeddings = await _embeddingGenerator.GenerateEmbeddingsAsync(chunk);
                var document = new Document
                {
                    Id = $"{sourceId}_{chunkIndex}",
                    Name = $"{sourceId}_chunk_{chunkIndex}",
                    Content = chunk,
                    Vector = embeddings,
                    Metadata = new Dictionary<string, string>
                    {
                        { "source_id", sourceId },
                        { "chunk_index", chunkIndex.ToString() }
                    }
                };
                documents.Add(document);
                chunkIndex++;
            }

            return documents;
        }
    }
} 