namespace CentralAlertas.Application.Notifications.Routing;

using CentralAlertas.Application.Notifications.Destinations;
public class UpdateRoutingRuleHandler
{
    private readonly IRoutingRuleRepository _repository;
    private readonly INotificationDestinationRepository _destinationRepository;

    public UpdateRoutingRuleHandler(
        IRoutingRuleRepository repository,
        INotificationDestinationRepository destinationRepository)
    {
        _repository = repository;
        _destinationRepository = destinationRepository;
    }

    public async Task<UpdateRoutingRuleHandlerResult> HandleAsync(
        UpdateRoutingRuleCommand command,
        CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(
            command.Id,
            cancellationToken);

        if (rule is null)
            return UpdateRoutingRuleHandlerResult.NotFound();

        var normalizedName = command.Name.Trim();
        var normalizedSeverity = NormalizeLower(command.Severity);
        var normalizedCategory = Normalize(command.Category);
        var normalizedType = Normalize(command.Type);
        var normalizedSource = Normalize(command.Source);
        var normalizedDeliveryMode = command.DeliveryMode.Trim().ToLower();

        var destinationIds = command.DestinationIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        var errors = RoutingRuleValidator.Validate(
            normalizedName,
            command.Order,
            normalizedSeverity,
            normalizedDeliveryMode,
            command.ThrottleMinutes,
            destinationIds);

        if (errors.Count > 0)
            return UpdateRoutingRuleHandlerResult.Failure(errors);

        var destinations = await _destinationRepository.GetByIdsAsync(
            destinationIds,
            cancellationToken);

        if (destinations.Count != destinationIds.Count)
        {
            return UpdateRoutingRuleHandlerResult.Failure(
            [
                "Um ou mais destinos informados não existem."
            ]);
        }

        rule.Update(
            normalizedName,
            command.Order,
            normalizedSeverity,
            normalizedCategory,
            normalizedType,
            normalizedSource,
            normalizedDeliveryMode,
            command.ThrottleMinutes,
            command.IsActive);

        rule.ReplaceDestinations(destinationIds);

        await _repository.SaveChangesAsync(cancellationToken);

        var result = RoutingRuleResult.FromEntity(rule);

        result.Destinations = destinations
            .Select(destination => new RoutingRuleDestinationResult
            {
                DestinationId = destination.Id,
                Name = destination.Name,
                Type = destination.Type
            })
            .ToList();

        return UpdateRoutingRuleHandlerResult.Success(result);
    }

    private static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }

    private static string? NormalizeLower(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().ToLower();
    }
}

public class UpdateRoutingRuleHandlerResult
{
    public bool IsSuccess { get; private set; }

    public bool WasNotFound { get; private set; }

    public List<string> Errors { get; private set; } = [];

    public RoutingRuleResult? Rule { get; private set; }

    public static UpdateRoutingRuleHandlerResult Success(RoutingRuleResult rule)
    {
        return new UpdateRoutingRuleHandlerResult
        {
            IsSuccess = true,
            Rule = rule
        };
    }

    public static UpdateRoutingRuleHandlerResult Failure(List<string> errors)
    {
        return new UpdateRoutingRuleHandlerResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }

    public static UpdateRoutingRuleHandlerResult NotFound()
    {
        return new UpdateRoutingRuleHandlerResult
        {
            IsSuccess = false,
            WasNotFound = true
        };
    }
}