namespace CentralAlertas.Api.Contracts.Dashboard;

public class DashboardSummaryResponse
{
    public int ActiveCriticalCount { get; set; }
    public int ActiveWarningCount { get; set; }
    public int ActiveInfoCount { get; set; }
    public int ActiveAlertsCount { get; set; }

    public int HealthySourcesCount { get; set; }
    public int SilentSourcesCount { get; set; }
    public int TotalSourcesCount { get; set; }

    public List<DashboardSeverityCountResponse> AlertsBySeverity { get; set; } = [];
    public List<DashboardCategoryCountResponse> AlertsByCategory { get; set; } = [];
    public List<DashboardLatestAlertResponse> LatestAlerts { get; set; } = [];
}

public class DashboardSeverityCountResponse
{
    public string Severity { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardCategoryCountResponse
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardLatestAlertResponse
{
    public Guid Id { get; set; }

    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public decimal? MetricValue { get; set; }
    public string? MetricUnit { get; set; }

    public DateTime LastSeenAt { get; set; }

    public bool IsEscalating { get; set; }
}