using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Destinations;

public class CreateNotificationDestinationHandler
{
    private readonly INotificationDestinationRepository _repository;

    public CreateNotificationDestinationHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateNotificationDestinationHandlerResult> HandleAsync(
        CreateNotificationDestinationCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedName = command.Name.Trim();
        var normalizedType = command.Type.Trim().ToLower();
        var configurationJson = command.ConfigurationJson.Trim();

        var errors = NotificationDestinationValidator.Validate(
            normalizedName,
            normalizedType,
            configurationJson);

        if (errors.Count > 0)
        {
            return CreateNotificationDestinationHandlerResult.Failure(errors);
        }

        var existing = await _repository.GetByNameAsync(
            normalizedName,
            cancellationToken);

        if (existing is not null)
        {
            return CreateNotificationDestinationHandlerResult.Failure(
            [
                "Já existe um destino com este nome."
            ]);
        }

        var destination = new NotificationDestination(
            normalizedName,
            normalizedType,
            configurationJson);

        await _repository.AddAsync(destination, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return CreateNotificationDestinationHandlerResult.Success(
            NotificationDestinationResult.FromEntity(destination));
    }
}

public class CreateNotificationDestinationHandlerResult
{
    public bool IsSuccess { get; private set; }

    public List<string> Errors { get; private set; } = [];

    public string? ErrorMessage => Errors.Count > 0 ? string.Join("; ", Errors) : null;

    public NotificationDestinationResult? Destination { get; private set; }

    public static CreateNotificationDestinationHandlerResult Success(
        NotificationDestinationResult destination)
    {
        return new CreateNotificationDestinationHandlerResult
        {
            IsSuccess = true,
            Destination = destination
        };
    }

    public static CreateNotificationDestinationHandlerResult Failure(
        List<string> errors)
    {
        return new CreateNotificationDestinationHandlerResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}