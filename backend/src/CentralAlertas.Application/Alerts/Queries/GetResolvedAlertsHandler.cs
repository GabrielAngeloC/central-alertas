using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class GetResolvedAlertsHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetResolvedAlertsHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<List<Alert>> HandleAsync(
        GetResolvedAlertsQuery query,
        CancellationToken cancellationToken)
    {
        return _alertRepository.GetResolvedAsync(
            query.Severity,
            query.Category,
            query.Source,
            cancellationToken);
    }
}