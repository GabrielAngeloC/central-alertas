using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Deliveries;

public class GetAlertDeliveriesHandler
{
    private readonly IAlertDeliveryRepository _repository;

    public GetAlertDeliveriesHandler(IAlertDeliveryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<AlertDelivery>> HandleAsync(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        return _repository.GetByAlertIdAsync(
            alertId,
            cancellationToken);
    }
}