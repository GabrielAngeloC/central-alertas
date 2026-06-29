using CentralAlertas.Application.Cors;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class AllowedOriginRepository : IAllowedOriginRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public AllowedOriginRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<AllowedOrigin>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _dbContext.AllowedOrigins
            .OrderBy(x => x.Origin)
            .ToListAsync(cancellationToken);
    }

    public Task<AllowedOrigin?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.AllowedOrigins
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<AllowedOrigin?> GetByOriginAsync(string origin, CancellationToken cancellationToken)
    {
        return _dbContext.AllowedOrigins
            .FirstOrDefaultAsync(x => x.Origin == origin, cancellationToken);
    }

    public async Task<List<string>> GetActiveOriginsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AllowedOrigins
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => x.Origin)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AllowedOrigin origin, CancellationToken cancellationToken)
    {
        await _dbContext.AllowedOrigins.AddAsync(origin, cancellationToken);
    }

    public void Remove(AllowedOrigin origin)
    {
        _dbContext.AllowedOrigins.Remove(origin);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
