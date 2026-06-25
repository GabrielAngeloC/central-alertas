using CentralAlertas.Application.Notifications.Destinations;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class NotificationDestinationRepository : INotificationDestinationRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public NotificationDestinationRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<NotificationDestination>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return _dbContext.NotificationDestinations
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<NotificationDestination?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.NotificationDestinations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<NotificationDestination>> GetByIdsAsync(
        List<Guid> ids,
        CancellationToken cancellationToken)
    {
        return _dbContext.NotificationDestinations
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<NotificationDestination?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return _dbContext.NotificationDestinations
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task AddAsync(
        NotificationDestination destination,
        CancellationToken cancellationToken)
    {
        await _dbContext.NotificationDestinations.AddAsync(
            destination,
            cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}