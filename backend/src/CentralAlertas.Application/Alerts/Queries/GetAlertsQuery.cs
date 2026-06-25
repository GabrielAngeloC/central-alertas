namespace CentralAlertas.Application.Alerts.Queries;

public class GetAlertsQuery
{
    public string Status { get; set; } = "active";

    public string? Severity { get; set; }

    public string? Category { get; set; }

    public string? Source { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}