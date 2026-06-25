namespace CentralAlertas.Application.Notifications.Destinations;

public class UpdateNotificationDestinationHandler
{
    private readonly INotificationDestinationRepository _repository;

    public UpdateNotificationDestinationHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateNotificationDestinationHandlerResult> HandleAsync(
        UpdateNotificationDestinationCommand command,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (destination is null)
        {
            return UpdateNotificationDestinationHandlerResult.NotFound();
        }

        var normalizedName = command.Name.Trim();
        var normalizedType = command.Type.Trim().ToLower();
        var configurationJson = command.ConfigurationJson.Trim();

        var errors = NotificationDestinationValidator.Validate(
            normalizedName,
            normalizedType,
            configurationJson);

        if (errors.Count > 0)
        {
            return UpdateNotificationDestinationHandlerResult.Failure(errors);
        }

        var existingWithSameName = await _repository.GetByNameAsync(
            normalizedName,
            cancellationToken);

        if (existingWithSameName is not null &&
            existingWithSameName.Id != destination.Id)
        {
            return UpdateNotificationDestinationHandlerResult.Failure(
            [
                "Já existe outro destino com este nome."
            ]);
        }

        destination.Update(
            normalizedName,
            normalizedType,
            configurationJson,
            command.IsActive);

        await _repository.SaveChangesAsync(cancellationToken);

        return UpdateNotificationDestinationHandlerResult.Success(
            NotificationDestinationResult.FromEntity(destination));
    }
}

public class UpdateNotificationDestinationHandlerResult
{
    public bool IsSuccess { get; private set; }

    public bool WasNotFound { get; private set; }

    public List<string> Errors { get; private set; } = [];

    public string? ErrorMessage => Errors.Count > 0 ? string.Join("; ", Errors) : null;

    public NotificationDestinationResult? Destination { get; private set; }

    public static UpdateNotificationDestinationHandlerResult Success(
        NotificationDestinationResult destination)
    {
        return new UpdateNotificationDestinationHandlerResult
        {
            IsSuccess = true,
            Destination = destination
        };
    }

    public static UpdateNotificationDestinationHandlerResult Failure(
        List<string> errors)
    {
        return new UpdateNotificationDestinationHandlerResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }

    public static UpdateNotificationDestinationHandlerResult NotFound()
    {
        return new UpdateNotificationDestinationHandlerResult
        {
            IsSuccess = false,
            WasNotFound = true,
            Errors = ["Destino de notificação não encontrado."]
        };
    }
}