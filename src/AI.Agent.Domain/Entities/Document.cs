using System.ComponentModel.DataAnnotations.Schema;

namespace AI.Agent.Domain.Entities;

public class Document
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Content { get; private set; }
    public string FileType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public bool IsProcessed { get; private set; }
    public string? ProcessingError { get; private set; }

    private Document() { } // For EF Core

    public Document(string name, string content, string fileType)
    {
        Id = Guid.NewGuid();
        Name = name;
        Content = content;
        FileType = fileType;
        CreatedAt = DateTime.UtcNow;
        IsProcessed = false;
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