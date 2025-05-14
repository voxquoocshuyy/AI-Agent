namespace AI.Agent.Domain.Entities;

/// <summary>
/// Represents a document in the system
/// </summary>
public class Document
{
    /// <summary>
    /// Unique identifier for the document
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Name of the document
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Content of the document
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Type of the document file (e.g., pdf, txt, csv)
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Vector embeddings of the document content
    /// </summary>
    public float[] Vector { get; set; } = Array.Empty<float>();

    /// <summary>
    /// Additional metadata about the document
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// When the document was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the document was last modified
    /// </summary>
    public DateTime LastModifiedAt { get; set; }

    /// <summary>
    /// Whether the document has been processed
    /// </summary>
    public bool IsProcessed { get; set; }

    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? ProcessingError { get; set; }

    private Document()
    {
    } // For EF Core

    public Document(string name, string content, string fileType)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Content = content;
        FileType = fileType;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
        IsProcessed = false;
        Metadata = new Dictionary<string, string>();
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        IsProcessed = false;
        ProcessingError = error;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent;
        LastModifiedAt = DateTime.UtcNow;
        IsProcessed = false;
    }
}