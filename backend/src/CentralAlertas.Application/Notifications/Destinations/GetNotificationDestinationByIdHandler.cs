namespace CentralAlertas.Application.Notifications.Destinations;

public class GetNotificationDestinationByIdHandler
{
    private readonly INotificationDestinationRepository _repository;

    public GetNotificationDestinationByIdHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationDestinationResult?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (destination is null)
            return null;

        return NotificationDestinationResult.FromEntity(destination);
    }
}