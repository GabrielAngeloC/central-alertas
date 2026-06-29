namespace CentralAlertas.Application.Notifications.Destinations;

public class DeleteNotificationDestinationHandler
{
    private readonly INotificationDestinationRepository _repository;

    public DeleteNotificationDestinationHandler(
        INotificationDestinationRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteNotificationDestinationResult> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var destination = await _repository.GetByIdAsync(id, cancellationToken);
        if (destination is null)
            return DeleteNotificationDestinationResult.NotFound();

        // A FK em routing_rule_destinations é Restrict: não dá para apagar
        // um destino em uso. Avisamos para o usuário desvincular antes.
        var inUse = await _repository.IsUsedByRoutingRuleAsync(id, cancellationToken);
        if (inUse)
            return DeleteNotificationDestinationResult.InUse();

        destination.Delete();
        await _repository.SaveChangesAsync(cancellationToken);

        return DeleteNotificationDestinationResult.Success();
    }
}

public class DeleteNotificationDestinationResult
{
    public bool IsSuccess { get; set; }
    public bool WasNotFound { get; set; }
    public bool WasInUse { get; set; }
    public string? ErrorMessage { get; set; }

    public static DeleteNotificationDestinationResult Success() =>
        new() { IsSuccess = true };

    public static DeleteNotificationDestinationResult NotFound() =>
        new() { WasNotFound = true, ErrorMessage = "Destino não encontrado." };

    public static DeleteNotificationDestinationResult InUse() =>
        new()
        {
            WasInUse = true,
            ErrorMessage = "Destino em uso por uma ou mais regras. Remova-o das regras antes de excluir."
        };
}
