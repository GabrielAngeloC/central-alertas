namespace CentralAlertas.Api.Contracts.Notifications.Routing;

public class TestRoutingRuleRequest
{
    public string? Source { get; set; }

    public string? Category { get; set; }

    public string? Type { get; set; }

    public string? Severity { get; set; }
}