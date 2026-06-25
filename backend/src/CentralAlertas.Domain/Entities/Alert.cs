namespace CentralAlertas.Domain.Entities;

public class Alert
{

    private readonly List<AlertOccurrence> _occurrences = [];

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Source { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;
    public string? Message { get; private set; }

    public string DedupKey { get; private set; } = string.Empty;

    public decimal? MetricValue { get; private set; }
    public string? MetricUnit { get; private set; }
    public decimal? MetricThreshold { get; private set; }

    public string? ItemsJson { get; private set; }
    public string? PayloadJson { get; private set; }

    public int OccurrenceCount { get; private set; } = 1;

    public DateTime FirstSeenAt { get; private set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastNotifiedAt { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public string? ResolutionReason { get; private set; }

    public IReadOnlyCollection<AlertOccurrence> Occurrences => _occurrences.AsReadOnly();

    public bool IsActive { get; private set; } = true;
    public bool IsEscalating { get; private set; } = false;

    private Alert()
    {
    }

    public void Resolve(string? reason)
    {
        IsActive = false;
        ResolvedAt = DateTime.UtcNow;
        ResolutionReason = reason;
    }


    public Alert(
        string source,
        string category,
        string type,
        string severity,
        string title,
        string? message,
        string dedupKey,
        decimal? metricValue,
        string? metricUnit,
        decimal? metricThreshold,
        string? itemsJson,
        string? payloadJson)
    {
        Source = source;
        Category = category;
        Type = type;
        Severity = severity;
        Title = title;
        Message = message;
        DedupKey = dedupKey;
        MetricValue = metricValue;
        MetricUnit = metricUnit;
        MetricThreshold = metricThreshold;
        ItemsJson = itemsJson;
        PayloadJson = payloadJson;
    }

    public void RegisterOccurrence(
        string severity,
        string title,
        string? message,
        decimal? metricValue,
        string? metricUnit,
        decimal? metricThreshold,
        string? itemsJson,
        string? payloadJson)
    {
        Severity = severity;
        Title = title;
        Message = message;

        var previousMetricValue = MetricValue;

        IsEscalating = metricValue.HasValue &&
                       previousMetricValue.HasValue &&
                       metricValue.Value > previousMetricValue.Value;

        MetricValue = metricValue;
        MetricUnit = metricUnit;
        MetricThreshold = metricThreshold;
        ItemsJson = itemsJson;
        PayloadJson = payloadJson;

        OccurrenceCount++;
        LastSeenAt = DateTime.UtcNow;
        IsActive = true;
        ResolvedAt = null;
        ResolutionReason = null;
    }

    public void MarkAsNotified()
    {
        LastNotifiedAt = DateTime.UtcNow;
    }

    public AlertOccurrence CreateOccurrence()
    {
        return new AlertOccurrence(
            Id,
            MetricValue,
            MetricUnit,
            MetricThreshold,
            ItemsJson,
            PayloadJson);
    }
}