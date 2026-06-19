namespace CentralAlertas.Application.Alerts.Ingestion;

public class CreateAlertCommand
{
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

    public string? ItemsJson { get; set; }
    public string? PayloadJson { get; set; }
}