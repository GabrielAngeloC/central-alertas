namespace CentralAlertas.Application.Alerts.Queries;

public class GetResolvedAlertsQuery
{
    public string? Severity { get; set; }

    public string? Category { get; set; }

    public string? Source { get; set; }
}