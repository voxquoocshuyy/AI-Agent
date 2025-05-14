using System.IO;
using System.Threading.Tasks;

namespace AI.Agent.Domain.Interfaces
{
    public interface IDocumentExtractor
    {
        Task<string> ExtractTextAsync(Stream fileStream, string fileType);
        bool IsFileTypeSupported(string fileType);
    }
} 