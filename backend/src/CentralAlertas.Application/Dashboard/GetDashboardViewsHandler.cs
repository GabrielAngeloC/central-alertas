namespace CentralAlertas.Application.Dashboard;

public class GetDashboardViewsHandler
{
    private readonly IDashboardRepository _dashboardRepository;

    public GetDashboardViewsHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public Task<List<DashboardView>> HandleAsync(CancellationToken cancellationToken)
    {
        return _dashboardRepository.GetViewsAsync(cancellationToken);
    }
}