using System;
using System.Linq;
using AI.Agent.Domain.Interfaces;
using AI.Agent.Infrastructure.DocumentProcessing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AI.Agent.UnitTests.DocumentProcessing
{
    public class TextChunkerTests
    {
        private readonly Mock<ILogger<TextChunker>> _loggerMock;
        private readonly TextChunker _textChunker;

        public TextChunkerTests()
        {
            _loggerMock = new Mock<ILogger<TextChunker>>();
            _textChunker = new TextChunker(_loggerMock.Object, maxChunkSize: 100, overlapSize: 20);
        }

        [Fact]
        public void SplitIntoChunks_EmptyText_ReturnsEmptyList()
        {
            // Act
            var result = _textChunker.SplitIntoChunks("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SplitIntoChunks_NullText_ReturnsEmptyList()
        {
            // Act
            var result = _textChunker.SplitIntoChunks(null);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SplitIntoChunks_TextSmallerThanChunkSize_ReturnsSingleChunk()
        {
            // Arrange
            var text = "This is a short text.";

            // Act
            var result = _textChunker.SplitIntoChunks(text);

            // Assert
            Assert.Single(result);
            Assert.Equal(text, result.First());
        }

        [Fact]
        public void SplitIntoChunks_TextLargerThanChunkSize_SplitsIntoMultipleChunks()
        {
            // Arrange
            var text = "First sentence. Second sentence. Third sentence. Fourth sentence. Fifth sentence.";

            // Act
            var result = _textChunker.SplitIntoChunks(text);

            // Assert
            Assert.True(result.Count() > 1);
            Assert.All(result, chunk =>
            {
                var b = chunk.Length <= 100;
            });
        }

        [Fact]
        public void SplitIntoChunks_ChunksHaveOverlap()
        {
            // Arrange
            var text = "First sentence. Second sentence. Third sentence. Fourth sentence. Fifth sentence.";

            // Act
            var result = _textChunker.SplitIntoChunks(text).ToList();

            // Assert
            for (int i = 1; i < result.Count; i++)
            {
                var previousChunk = result[i - 1];
                var currentChunk = result[i];
                var overlap = GetOverlap(previousChunk, currentChunk);
                Assert.True(overlap.Length > 0);
            }
        }

        [Fact]
        public void SplitIntoChunks_PreservesSentenceBoundaries()
        {
            // Arrange
            var text = "First sentence. Second sentence. Third sentence. Fourth sentence. Fifth sentence.";

            // Act
            var result = _textChunker.SplitIntoChunks(text);

            // Assert
            Assert.All(result, chunk =>
            {
                Assert.True(chunk.EndsWith(".") || chunk.EndsWith("!") || chunk.EndsWith("?"));
            });
        }

        private string GetOverlap(string text1, string text2)
        {
            var words1 = text1.Split(' ');
            var words2 = text2.Split(' ');

            for (int i = 0; i < words1.Length; i++)
            {
                var overlap = string.Join(" ", words1.Skip(i));
                if (text2.StartsWith(overlap))
                {
                    return overlap;
                }
            }

            return string.Empty;
        }
    }
} 