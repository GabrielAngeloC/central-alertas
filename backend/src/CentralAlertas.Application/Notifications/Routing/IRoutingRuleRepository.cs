using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Routing;

public interface IRoutingRuleRepository
{
    Task<List<RoutingRule>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<List<RoutingRule>> GetActiveWithDestinationsAsync(
        CancellationToken cancellationToken);

    Task<RoutingRule?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(
        RoutingRule rule,
        CancellationToken cancellationToken);

    void Remove(RoutingRule rule);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}