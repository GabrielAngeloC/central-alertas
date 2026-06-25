namespace CentralAlertas.Application.Notifications.Destinations;

public class GetNotificationDestinationsHandler
{
    private readonly INotificationDestinationRepository _repository;

    public GetNotificationDestinationsHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<NotificationDestinationResult>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var destinations = await _repository.GetAllAsync(cancellationToken);

        return destinations
            .Select(NotificationDestinationResult.FromEntity)
            .ToList();
    }
}