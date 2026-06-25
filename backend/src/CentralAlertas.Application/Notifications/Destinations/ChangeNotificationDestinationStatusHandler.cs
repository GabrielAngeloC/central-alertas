namespace CentralAlertas.Application.Notifications.Destinations;

public class ChangeNotificationDestinationStatusHandler
{
    private readonly INotificationDestinationRepository _repository;

    public ChangeNotificationDestinationStatusHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChangeNotificationDestinationStatusResult> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (destination is null)
            return ChangeNotificationDestinationStatusResult.NotFound();

        destination.Activate();

        await _repository.SaveChangesAsync(cancellationToken);

        return ChangeNotificationDestinationStatusResult.Success(
            NotificationDestinationResult.FromEntity(destination));
    }

    public async Task<ChangeNotificationDestinationStatusResult> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (destination is null)
            return ChangeNotificationDestinationStatusResult.NotFound();

        destination.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);

        return ChangeNotificationDestinationStatusResult.Success(
            NotificationDestinationResult.FromEntity(destination));
    }
}

public class ChangeNotificationDestinationStatusResult
{
    public bool IsSuccess { get; set; }

    public bool WasNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public NotificationDestinationResult? Destination { get; set; }

    public static ChangeNotificationDestinationStatusResult Success(
        NotificationDestinationResult destination)
    {
        return new ChangeNotificationDestinationStatusResult
        {
            IsSuccess = true,
            Destination = destination
        };
    }

    public static ChangeNotificationDestinationStatusResult NotFound()
    {
        return new ChangeNotificationDestinationStatusResult
        {
            IsSuccess = false,
            WasNotFound = true,
            ErrorMessage = "Destino de notificação não encontrado."
        };
    }
}