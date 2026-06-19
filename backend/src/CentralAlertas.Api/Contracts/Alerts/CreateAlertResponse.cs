namespace CentralAlertas.Api.Contracts.Alerts;

public class CreateAlertResponse
{
    public Guid AlertId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
    public bool IsNewAlert { get; set; }
    public bool IsEscalating { get; set; }
}