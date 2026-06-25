namespace CentralAlertas.Domain.Entities;

public class RoutingRuleDestination
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid RoutingRuleId { get; private set; }
    public RoutingRule RoutingRule { get; private set; } = null!;

    public Guid NotificationDestinationId { get; private set; }
    public NotificationDestination NotificationDestination { get; private set; } = null!;

    private RoutingRuleDestination()
    {
    }

    public RoutingRuleDestination(
        Guid routingRuleId,
        Guid notificationDestinationId)
    {
        RoutingRuleId = routingRuleId;
        NotificationDestinationId = notificationDestinationId;
    }
}