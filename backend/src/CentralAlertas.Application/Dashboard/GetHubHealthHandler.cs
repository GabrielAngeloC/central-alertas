namespace CentralAlertas.Application.Dashboard;

public class GetHubHealthHandler
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetHubHealthHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<DashboardHubHealth> HandleAsync(CancellationToken cancellationToken)
    {
        return _dashboardRepository.GetHubHealthAsync(cancellationToken);
    }
}
