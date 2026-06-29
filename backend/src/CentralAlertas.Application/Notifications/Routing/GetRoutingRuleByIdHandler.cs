namespace CentralAlertas.Application.Notifications.Routing;

public class GetRoutingRuleByIdHandler
{
    private readonly IRoutingRuleRepository _repository;

    public GetRoutingRuleByIdHandler(IRoutingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<RoutingRuleResult?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (rule is null)
            return null;

        return RoutingRuleResult.FromEntity(rule);
    }
}
