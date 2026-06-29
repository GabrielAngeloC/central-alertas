namespace CentralAlertas.Application.Dashboard;

public class GetDashboardStatisticsHandler
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardStatisticsHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<DashboardStatistics> HandleAsync(CancellationToken cancellationToken)
    {
        return _dashboardRepository.GetStatisticsAsync(cancellationToken);
    }
}
