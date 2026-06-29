namespace CentralAlertas.Application.Dashboard;

// Saúde do hub: agregado de entregas (Telegram/e-mail) nas últimas N horas.
public class DashboardHubHealth
{
    public int WindowHours { get; set; } = 24;

    public int TotalDeliveries { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }

    public List<DashboardChannelHealth> ByChannel { get; set; } = [];
}

public class DashboardChannelHealth
{
    public string Channel { get; set; } = string.Empty;
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Skipped { get; set; }
}
