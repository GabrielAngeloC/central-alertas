namespace CentralAlertas.Application.Notifications.Routing;

public class ChangeRoutingRuleStatusHandler
{
    private readonly IRoutingRuleRepository _repository;

    public ChangeRoutingRuleStatusHandler(IRoutingRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChangeRoutingRuleStatusResult> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (rule is null)
            return ChangeRoutingRuleStatusResult.NotFound();

        rule.Activate();

        await _repository.SaveChangesAsync(cancellationToken);

        return ChangeRoutingRuleStatusResult.Success(
            RoutingRuleResult.FromEntity(rule));
    }

    public async Task<ChangeRoutingRuleStatusResult> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var rule = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (rule is null)
            return ChangeRoutingRuleStatusResult.NotFound();

        rule.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);

        return ChangeRoutingRuleStatusResult.Success(
            RoutingRuleResult.FromEntity(rule));
    }
}

public class ChangeRoutingRuleStatusResult
{
    public bool IsSuccess { get; set; }

    public bool WasNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public RoutingRuleResult? Rule { get; set; }

    public static ChangeRoutingRuleStatusResult Success(
        RoutingRuleResult rule)
    {
        return new ChangeRoutingRuleStatusResult
        {
            IsSuccess = true,
            Rule = rule
        };
    }

    public static ChangeRoutingRuleStatusResult NotFound()
    {
        return new ChangeRoutingRuleStatusResult
        {
            IsSuccess = false,
            WasNotFound = true,
            ErrorMessage = "Regra de roteamento não encontrada."
        };
    }
}
