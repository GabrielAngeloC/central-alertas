using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Deliveries;

public interface IAlertDeliveryRepository
{
    Task AddAsync(
        AlertDelivery delivery,
        CancellationToken cancellationToken);

    Task<List<AlertDelivery>> GetByAlertIdAsync(
        Guid alertId,
        CancellationToken cancellationToken);

    Task<AlertDelivery?> GetLastSuccessfulDeliveryAsync(
    Guid alertId,
    Guid notificationDestinationId,
    CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}