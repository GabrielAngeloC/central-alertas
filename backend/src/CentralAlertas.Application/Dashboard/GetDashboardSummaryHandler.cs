namespace CentralAlertas.Application.Dashboard;

public class GetDashboardSummaryHandler
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardSummaryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<DashboardSummary> HandleAsync(CancellationToken cancellationToken)
    {
        return _dashboardRepository.GetSummaryAsync(cancellationToken);
    }
}