using System.Text.Json;

namespace CentralAlertas.Api.Contracts.Alerts;

public class AlertOccurrenceResponse
{
    public Guid Id { get; set; }

    public Guid AlertId { get; set; }

    public decimal? MetricValue { get; set; }
    public string? MetricUnit { get; set; }
    public decimal? MetricThreshold { get; set; }

    public JsonElement? Items { get; set; }
    public JsonElement? Payload { get; set; }

    public DateTime ReceivedAt { get; set; }
}