using CentralAlertas.Application.Dashboard;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class DashboardViewRepository : IDashboardViewRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public DashboardViewRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<DashboardViewConfig>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _dbContext.DashboardViews
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public Task<List<DashboardViewConfig>> GetActiveOrderedAsync(CancellationToken cancellationToken)
    {
        return _dbContext.DashboardViews
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public Task<DashboardViewConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.DashboardViews
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<DashboardViewConfig?> GetByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        return _dbContext.DashboardViews
            .FirstOrDefaultAsync(x => x.Category == category, cancellationToken);
    }

    public async Task AddAsync(DashboardViewConfig view, CancellationToken cancellationToken)
    {
        await _dbContext.DashboardViews.AddAsync(view, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
