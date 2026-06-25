namespace CentralAlertas.Api.Contracts.Notifications.Deliveries;

public class AlertDeliveryResponse
{
    public Guid Id { get; set; }

    public Guid AlertId { get; set; }

    public Guid? RoutingRuleId { get; set; }

    public string? RoutingRuleName { get; set; }

    public Guid? NotificationDestinationId { get; set; }

    public string? NotificationDestinationName { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public DateTime AttemptedAt { get; set; }

    public DateTime? SentAt { get; set; }
}