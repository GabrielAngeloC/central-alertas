namespace CentralAlertas.Application.Notifications.Routing;

public class CreateRoutingRuleCommand
{
    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public string? Severity { get; set; }

    public string? Category { get; set; }

    public string? Type { get; set; }

    public string? Source { get; set; }

    public string DeliveryMode { get; set; } = "immediate";

    public int? ThrottleMinutes { get; set; }

    public List<Guid> DestinationIds { get; set; } = [];
}