namespace CentralAlertas.Domain.Entities;

public class AlertOccurrence
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AlertId { get; private set; }
    public Alert Alert { get; private set; } = null!;

    public decimal? MetricValue { get; private set; }
    public string? MetricUnit { get; private set; }
    public decimal? MetricThreshold { get; private set; }

    public string? ItemsJson { get; private set; }
    public string? PayloadJson { get; private set; }

    public DateTime ReceivedAt { get; private set; } = DateTime.UtcNow;

    private AlertOccurrence()
    {
    }

    public AlertOccurrence(
        Guid alertId,
        decimal? metricValue,
        string? metricUnit,
        decimal? metricThreshold,
        string? itemsJson,
        string? payloadJson)
    {
        AlertId = alertId;
        MetricValue = metricValue;
        MetricUnit = metricUnit;
        MetricThreshold = metricThreshold;
        ItemsJson = itemsJson;
        PayloadJson = payloadJson;
    }
}   