using AI.Agent.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AI.Agent.UnitTests.Domain.Entities;

public class DocumentTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateDocument()
    {
        // Arrange
        var name = "test.pdf";
        var content = "test content";
        var fileType = "pdf";

        // Act
        var document = new Document(name, content, fileType);

        // Assert
        document.Id.Should().NotBe(Guid.Empty.ToString());
        document.Name.Should().Be(name);
        document.Content.Should().Be(content);
        document.FileType.Should().Be(fileType);
        document.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        document.IsProcessed.Should().BeFalse();
        document.ProcessingError.Should().BeNull();
    }

    [Fact]
    public void MarkAsProcessed_ShouldUpdateStatus()
    {
        // Arrange
        var document = new Document("test.pdf", "content", "pdf");

        // Act
        document.MarkAsProcessed();

        // Assert
        document.IsProcessed.Should().BeTrue();
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatusAndError()
    {
        // Arrange
        var document = new Document("test.pdf", "content", "pdf");
        var error = "Processing failed";

        // Act
        document.MarkAsFailed(error);

        // Assert
        document.IsProcessed.Should().BeFalse();
        document.ProcessingError.Should().Be(error);
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateContent_ShouldResetProcessingStatus()
    {
        // Arrange
        var document = new Document("test.pdf", "old content", "pdf");
        document.MarkAsProcessed();
        var newContent = "new content";

        // Act
        document.UpdateContent(newContent);

        // Assert
        document.Content.Should().Be(newContent);
        document.IsProcessed.Should().BeFalse();
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        document.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}