namespace CentralAlertas.Api.Contracts.Notifications.Routing;

public class UpdateRoutingRuleRequest
{
    public string? Name { get; set; }

    public int Order { get; set; }

    public string? Severity { get; set; }

    public string? Category { get; set; }

    public string? Type { get; set; }

    public string? Source { get; set; }

    public string? DeliveryMode { get; set; }

    public int? ThrottleMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    public List<Guid> DestinationIds { get; set; } = [];
}