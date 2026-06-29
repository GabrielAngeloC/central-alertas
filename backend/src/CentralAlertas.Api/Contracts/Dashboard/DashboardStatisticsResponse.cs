namespace CentralAlertas.Api.Contracts.Dashboard;

public class DashboardStatisticsResponse
{
    public List<DashboardDailyCountResponse> AlertsPerDay { get; set; } = [];
    public List<DashboardLabelCountResponse> ByCategory { get; set; } = [];
    public List<DashboardLabelCountResponse> ByType { get; set; } = [];
}

public class DashboardDailyCountResponse
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class DashboardLabelCountResponse
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}
