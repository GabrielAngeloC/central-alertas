using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class GetAlertOccurrencesHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertOccurrencesHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<List<AlertOccurrence>> HandleAsync(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        return _alertRepository.GetOccurrencesAsync(alertId, cancellationToken);
    }
}