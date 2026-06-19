using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class GetAlertByIdHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertByIdHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<Alert?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _alertRepository.GetByIdAsync(id, cancellationToken);
    }
}