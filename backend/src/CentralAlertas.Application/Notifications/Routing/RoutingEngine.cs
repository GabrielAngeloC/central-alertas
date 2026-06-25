namespace CentralAlertas.Application.Notifications.Routing;

public class RoutingEngine
{
    private readonly IRoutingRuleRepository _routingRuleRepository;

    public RoutingEngine(IRoutingRuleRepository routingRuleRepository)
    {
        _routingRuleRepository = routingRuleRepository;
    }

    public async Task<RoutingDecision> RouteAsync(
        RouteAlertCommand command,
        CancellationToken cancellationToken)
    {
        var rules = await _routingRuleRepository.GetActiveWithDestinationsAsync(
            cancellationToken);

        var normalizedSeverity = NormalizeLower(command.Severity);
        var normalizedCategory = Normalize(command.Category);
        var normalizedType = Normalize(command.Type);
        var normalizedSource = Normalize(command.Source);

        var matchedRule = rules.FirstOrDefault(rule =>
            rule.Matches(
                normalizedSeverity,
                normalizedCategory,
                normalizedType,
                normalizedSource));

        if (matchedRule is null)
            return RoutingDecision.NoMatch();

        return new RoutingDecision
        {
            Matched = true,
            RuleId = matchedRule.Id,
            RuleName = matchedRule.Name,
            DeliveryMode = matchedRule.DeliveryMode,
            ThrottleMinutes = matchedRule.ThrottleMinutes,
            Destinations = matchedRule.Destinations
                .Where(destination => destination.NotificationDestination is not null)
                .Where(destination => destination.NotificationDestination.IsActive)
                .Select(destination => new RoutingDecisionDestination
                {
                    DestinationId = destination.NotificationDestinationId,
                    Name = destination.NotificationDestination.Name,
                    Type = destination.NotificationDestination.Type,
                    ConfigurationJson = destination.NotificationDestination.ConfigurationJson
                })
                .ToList()
        };
    }

    private static string Normalize(string value)
    {
        return value.Trim();
    }

    private static string NormalizeLower(string value)
    {
        return value.Trim().ToLower();
    }
}