using System.Text.Json.Serialization;

namespace AI.Agent.Infrastructure.VectorDatabase;

public class SearchDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("vector")]
    public float[] Vector { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class SearchResult
{
    public string Id { get; set; }
    public string Content { get; set; }
    public float Score { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
} 