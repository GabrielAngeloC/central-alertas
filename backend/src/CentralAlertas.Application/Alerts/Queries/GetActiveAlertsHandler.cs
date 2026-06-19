using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class GetActiveAlertsHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetActiveAlertsHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<List<Alert>> HandleAsync(
        GetActiveAlertsQuery query,
        CancellationToken cancellationToken)
    {
        return _alertRepository.GetActiveAsync(
            query.Severity,
            query.Category,
            query.Source,
            cancellationToken);
    }
}