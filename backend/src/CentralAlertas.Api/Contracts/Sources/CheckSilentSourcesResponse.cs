namespace CentralAlertas.Api.Contracts.Sources;

public class CheckSilentSourcesResponse
{
    public int CheckedSourcesCount { get; set; }
    public int SilentSourcesCount { get; set; }

    public List<CheckSilentSourceAlertResponse> Alerts { get; set; } = [];
}

public class CheckSilentSourceAlertResponse
{
    public string SourceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MinutesLate { get; set; }

    public Guid AlertId { get; set; }
    public string AlertStatus { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
}