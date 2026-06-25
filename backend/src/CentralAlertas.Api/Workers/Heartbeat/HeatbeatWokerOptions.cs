namespace CentralAlertas.Api.Workers.Heartbeat;

public class HeartbeatWorkerOptions
{
    public bool Enabled { get; set; } = true;

    public int IntervalSeconds { get; set; } = 60;
}