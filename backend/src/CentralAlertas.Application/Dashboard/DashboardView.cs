using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Dashboard;

public class DashboardView
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }

    public List<Alert> Alerts { get; set; } = [];
}