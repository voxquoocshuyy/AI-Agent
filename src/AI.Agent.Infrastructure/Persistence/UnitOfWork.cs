using AI.Agent.Domain.Repositories;
using AI.Agent.Infrastructure.Persistence.Repositories;

namespace AI.Agent.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDocumentRepository? _documentRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IDocumentRepository Documents => _documentRepository ??= new DocumentRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 