using CentralAlertas.Application.Notifications.Deliveries;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class AlertDeliveryRepository : IAlertDeliveryRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public AlertDeliveryRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        AlertDelivery delivery,
        CancellationToken cancellationToken)
    {
        await _dbContext.AlertDeliveries.AddAsync(
            delivery,
            cancellationToken);
    }

    public Task<List<AlertDelivery>> GetByAlertIdAsync(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        return _dbContext.AlertDeliveries
            .AsNoTracking()
            .Include(x => x.RoutingRule)
            .Include(x => x.NotificationDestination)
            .Where(x => x.AlertId == alertId)
            .OrderByDescending(x => x.AttemptedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<AlertDelivery?> GetLastSuccessfulDeliveryAsync(
    Guid alertId,
    Guid notificationDestinationId,
    CancellationToken cancellationToken)
    {
        return _dbContext.AlertDeliveries
            .AsNoTracking()
            .Where(x => x.AlertId == alertId)
            .Where(x => x.NotificationDestinationId == notificationDestinationId)
            .Where(x => x.Status == "success")
            .OrderByDescending(x => x.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}