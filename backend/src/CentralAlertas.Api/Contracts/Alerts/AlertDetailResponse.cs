using System.Text.Json;

namespace CentralAlertas.Api.Contracts.Alerts;

public class AlertDetailResponse
{
    public Guid Id { get; set; }

    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }

    public string DedupKey { get; set; } = string.Empty;

    public decimal? MetricValue { get; set; }
    public string? MetricUnit { get; set; }
    public decimal? MetricThreshold { get; set; }

    public JsonElement? Items { get; set; }
    public JsonElement? Payload { get; set; }

    public int OccurrenceCount { get; set; }

    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime? LastNotifiedAt { get; set; }

    public bool IsActive { get; set; }
    public bool IsEscalating { get; set; }
}