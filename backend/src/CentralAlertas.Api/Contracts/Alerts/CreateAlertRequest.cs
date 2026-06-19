using System.Text.Json;
using System.Text.Json.Serialization;

namespace CentralAlertas.Api.Contracts.Alerts;

public class CreateAlertRequest
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("dedup_key")]
    public string? DedupKey { get; set; }

    [JsonPropertyName("metric")]
    public AlertMetricRequest? Metric { get; set; }

    [JsonPropertyName("items")]
    public JsonElement? Items { get; set; }

    [JsonPropertyName("payload")]
    public JsonElement? Payload { get; set; }
}

public class AlertMetricRequest
{
    [JsonPropertyName("value")]
    public decimal? Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("threshold")]
    public decimal? Threshold { get; set; }
}