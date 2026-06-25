using CentralAlertas.Domain.Entities;
using CentralAlertas.Application.Notifications.Destinations;

namespace CentralAlertas.Application.Notifications.Routing;

public class CreateRoutingRuleHandler
{
    private readonly IRoutingRuleRepository _repository;
    private readonly INotificationDestinationRepository _destinationRepository;

    public CreateRoutingRuleHandler(
        IRoutingRuleRepository repository,
        INotificationDestinationRepository destinationRepository)
    {
        _repository = repository;
        _destinationRepository = destinationRepository;
    }
    public async Task<CreateRoutingRuleHandlerResult> HandleAsync(
        CreateRoutingRuleCommand command,
        CancellationToken cancellationToken)
    {
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
            return CreateRoutingRuleHandlerResult.Failure(errors);

        var destinations = await _destinationRepository.GetByIdsAsync(
            destinationIds,
            cancellationToken);

        if (destinations.Count != destinationIds.Count)
        {
            return CreateRoutingRuleHandlerResult.Failure(
            [
                "Um ou mais destinos informados não existem."
            ]);
        }

        var rule = new RoutingRule(
            normalizedName,
            command.Order,
            normalizedSeverity,
            normalizedCategory,
            normalizedType,
            normalizedSource,
            normalizedDeliveryMode,
            command.ThrottleMinutes);

        rule.ReplaceDestinations(destinationIds);

        await _repository.AddAsync(rule, cancellationToken);
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

        return CreateRoutingRuleHandlerResult.Success(result);
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

public class CreateRoutingRuleHandlerResult
{
    public bool IsSuccess { get; private set; }

    public List<string> Errors { get; private set; } = [];

    public RoutingRuleResult? Rule { get; private set; }

    public static CreateRoutingRuleHandlerResult Success(RoutingRuleResult rule)
    {
        return new CreateRoutingRuleHandlerResult
        {
            IsSuccess = true,
            Rule = rule
        };
    }

    public static CreateRoutingRuleHandlerResult Failure(List<string> errors)
    {
        return new CreateRoutingRuleHandlerResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}