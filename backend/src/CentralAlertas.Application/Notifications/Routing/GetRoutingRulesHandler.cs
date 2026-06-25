namespace CentralAlertas.Application.Notifications.Routing;

public class GetRoutingRulesHandler
{
    private readonly IRoutingRuleRepository _repository;

    public GetRoutingRulesHandler(IRoutingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RoutingRuleResult>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var rules = await _repository.GetAllAsync(cancellationToken);

        return rules
            .Select(RoutingRuleResult.FromEntity)
            .ToList();
    }
}