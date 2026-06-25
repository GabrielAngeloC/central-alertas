namespace CentralAlertas.Domain.Entities;

public class AlertDelivery
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AlertId { get; private set; }
    public Alert Alert { get; private set; } = null!;

    public Guid? RoutingRuleId { get; private set; }
    public RoutingRule? RoutingRule { get; private set; }

    public Guid? NotificationDestinationId { get; private set; }
    public NotificationDestination? NotificationDestination { get; private set; }

    public string Channel { get; private set; } = string.Empty;

    public string Status { get; private set; } = string.Empty;

    public string? ErrorMessage { get; private set; }

    public DateTime AttemptedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? SentAt { get; private set; }

    private AlertDelivery()
    {
    }

    public AlertDelivery(
        Guid alertId,
        Guid? routingRuleId,
        Guid? notificationDestinationId,
        string channel,
        string status,
        string? errorMessage = null)
    {
        AlertId = alertId;
        RoutingRuleId = routingRuleId;
        NotificationDestinationId = notificationDestinationId;
        Channel = channel;
        Status = status;
        ErrorMessage = errorMessage;

        if (status == "success")
            SentAt = DateTime.UtcNow;
    }

    public void MarkSuccess()
    {
        Status = "success";
        SentAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkFailed(string errorMessage)
    {
        Status = "failed";
        ErrorMessage = errorMessage;
        SentAt = null;
    }

    public void MarkSkipped(string reason)
    {
        Status = "skipped";
        ErrorMessage = reason;
        SentAt = null;
    }
}