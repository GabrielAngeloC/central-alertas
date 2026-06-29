namespace CentralAlertas.Api.Contracts.Dashboard;

public class HubHealthResponse
{
    public int WindowHours { get; set; }

    public int TotalDeliveries { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }

    public List<ChannelHealthResponse> ByChannel { get; set; } = [];
}

public class ChannelHealthResponse
{
    public string Channel { get; set; } = string.Empty;
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Skipped { get; set; }
}
