using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using AI.Agent.Domain.Entities;

namespace AI.Agent.Domain.Interfaces
{
    public interface IDocumentProcessor
    {
        Task<Document> ProcessAsync(Stream fileStream, string fileName, string fileType);
        Task<IEnumerable<Document>> ChunkAsync(Document document);
    }
} 