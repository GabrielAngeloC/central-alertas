using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Dashboard;

public class DashboardSummary
{
    public int ActiveCriticalCount { get; set; }
    public int ActiveWarningCount { get; set; }
    public int ActiveInfoCount { get; set; }
    public int ActiveAlertsCount { get; set; }

    public int HealthySourcesCount { get; set; }
    public int SilentSourcesCount { get; set; }
    public int TotalSourcesCount { get; set; }

    public List<DashboardSeverityCount> AlertsBySeverity { get; set; } = [];
    public List<DashboardCategoryCount> AlertsByCategory { get; set; } = [];
    public List<Alert> LatestAlerts { get; set; } = [];
}

public class DashboardSeverityCount
{
    public string Severity { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardCategoryCount
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}