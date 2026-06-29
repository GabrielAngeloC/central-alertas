namespace CentralAlertas.Application.Dashboard;

public class DashboardStatistics
{
    // Tendência: novos alertas por dia nos últimos 30 dias.
    public List<DashboardDailyCount> AlertsPerDay { get; set; } = [];

    // Distribuição por categoria e por tipo (últimos 30 dias).
    public List<DashboardLabelCount> ByCategory { get; set; } = [];
    public List<DashboardLabelCount> ByType { get; set; } = [];
}

public class DashboardDailyCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class DashboardLabelCount
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}
