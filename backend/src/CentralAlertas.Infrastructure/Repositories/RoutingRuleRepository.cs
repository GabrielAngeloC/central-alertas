using CentralAlertas.Application.Notifications.Routing;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class RoutingRuleRepository : IRoutingRuleRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public RoutingRuleRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<RoutingRule>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.RoutingRules
            .AsNoTracking()
            .Include(x => x.Destinations)
            .ThenInclude(x => x.NotificationDestination)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<List<RoutingRule>> GetActiveWithDestinationsAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.RoutingRules
            .AsNoTracking()
            .Include(x => x.Destinations)
            .ThenInclude(x => x.NotificationDestination)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<RoutingRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.RoutingRules
            .Include(x => x.Destinations)
            .ThenInclude(x => x.NotificationDestination)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        RoutingRule rule,
        CancellationToken cancellationToken)
    {
        await _dbContext.RoutingRules.AddAsync(rule, cancellationToken);
    }

    public void Remove(RoutingRule rule)
    {
        _dbContext.RoutingRules.Remove(rule);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}