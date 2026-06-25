namespace CentralAlertas.Application.Notifications.Dispatching;

public class NotificationMessage
{
    public Guid AlertId { get; set; }

    public string Source { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Message { get; set; }

    public decimal? MetricValue { get; set; }

    public string? MetricUnit { get; set; }

    public decimal? MetricThreshold { get; set; }

    public int OccurrenceCount { get; set; }

    public bool IsEscalating { get; set; }

    public DateTime LastSeenAt { get; set; }
}