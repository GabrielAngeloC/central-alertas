namespace CentralAlertas.Application.Alerts.Queries;

public class GetActiveAlertsQuery
{
    public string? Severity { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
}