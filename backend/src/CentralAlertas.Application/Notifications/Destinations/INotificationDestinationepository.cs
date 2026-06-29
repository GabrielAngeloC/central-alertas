using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Destinations;

public interface INotificationDestinationRepository
{
    Task<List<NotificationDestination>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<NotificationDestination?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<NotificationDestination>> GetByIdsAsync(
        List<Guid> ids,
        CancellationToken cancellationToken);

    Task<NotificationDestination?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task AddAsync(
        NotificationDestination destination,
        CancellationToken cancellationToken);

    void Remove(NotificationDestination destination);

    // Indica se o destino está vinculado a alguma regra de roteamento.
    Task<bool> IsUsedByRoutingRuleAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}