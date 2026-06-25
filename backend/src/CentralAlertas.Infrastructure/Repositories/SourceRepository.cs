using CentralAlertas.Application.Sources;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public SourceRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Source>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Sources
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Source?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.Sources
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Source?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return _dbContext.Sources
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task AddAsync(
        Source source,
        CancellationToken cancellationToken)
    {
        await _dbContext.Sources.AddAsync(source, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}