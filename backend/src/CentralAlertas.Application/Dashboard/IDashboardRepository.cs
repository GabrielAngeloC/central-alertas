namespace CentralAlertas.Application.Dashboard;

public interface IDashboardRepository
{
    Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken);

    Task<List<DashboardView>> GetViewsAsync(CancellationToken cancellationToken);

    Task<DashboardStatistics> GetStatisticsAsync(CancellationToken cancellationToken);

    Task<DashboardHubHealth> GetHubHealthAsync(CancellationToken cancellationToken);
}