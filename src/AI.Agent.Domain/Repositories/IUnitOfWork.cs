namespace AI.Agent.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IDocumentRepository Documents { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 