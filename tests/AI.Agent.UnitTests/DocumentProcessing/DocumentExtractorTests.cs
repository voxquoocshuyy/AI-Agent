using System.Text;
using AI.Agent.Infrastructure.DocumentProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.Agent.UnitTests.DocumentProcessing
{
    public class DocumentExtractorTests
    {
        private readonly Mock<ILogger<DocumentExtractor>> _loggerMock;
        private readonly DocumentExtractor _documentExtractor;

        public DocumentExtractorTests()
        {
            _loggerMock = new Mock<ILogger<DocumentExtractor>>();
            // _documentExtractor = new DocumentExtractor(_loggerMock.Object);
        }

        [Fact]
        public async Task ExtractTextAsync_NullStream_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _documentExtractor.ExtractTextAsync(null, "test.txt"));
        }

        [Fact]
        public async Task ExtractTextAsync_NullFileName_ThrowsArgumentNullException()
        {
            // Arrange
            using var stream = new MemoryStream();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _documentExtractor.ExtractTextAsync(stream, null));
        }

        [Fact]
        public async Task ExtractTextAsync_EmptyStream_ReturnsEmptyString()
        {
            // Arrange
            using var stream = new MemoryStream();

            // Act
            var result = await _documentExtractor.ExtractTextAsync(stream, "test.txt");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExtractTextAsync_TextFile_ExtractsContent()
        {
            // Arrange
            var content = "This is a test text file content.";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = await _documentExtractor.ExtractTextAsync(stream, "test.txt");

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task ExtractTextAsync_UnsupportedFileType_ThrowsNotSupportedException()
        {
            // Arrange
            using var stream = new MemoryStream();

            // Act & Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                _documentExtractor.ExtractTextAsync(stream, "test.xyz"));
        }

        [Fact]
        public async Task ExtractTextAsync_StreamDisposed_ThrowsObjectDisposedException()
        {
            // Arrange
            var stream = new MemoryStream();
            stream.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() =>
                _documentExtractor.ExtractTextAsync(stream, "test.txt"));
        }

        [Fact]
        public async Task ExtractTextAsync_StreamNotReadable_ThrowsInvalidOperationException()
        {
            // Arrange
            using var stream = new MemoryStream();
            stream.Close();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _documentExtractor.ExtractTextAsync(stream, "test.txt"));
        }
    }
}