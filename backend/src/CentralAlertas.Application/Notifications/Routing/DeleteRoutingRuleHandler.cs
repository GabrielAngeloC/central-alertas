namespace CentralAlertas.Application.Notifications.Routing;

public class DeleteRoutingRuleHandler
{
    private readonly IRoutingRuleRepository _repository;

    public DeleteRoutingRuleHandler(IRoutingRuleRepository repository)
    {
        _repository = repository;
    }

    // Remove a regra. O histórico em alert_deliveries é preservado
    // (FK RoutingRuleId vira null); os vínculos em routing_rule_destinations
    // são removidos em cascata.
    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(id, cancellationToken);
        if (rule is null)
            return false;

        rule.Delete();
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
