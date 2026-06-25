namespace CentralAlertas.Api.Contracts.Notifications.Routing;

public class RoutingDecisionResponse
{
    public bool Matched { get; set; }

    public Guid? RuleId { get; set; }

    public string? RuleName { get; set; }

    public string DeliveryMode { get; set; } = string.Empty;

    public int? ThrottleMinutes { get; set; }

    public List<RoutingDecisionDestinationResponse> Destinations { get; set; } = [];
}

public class RoutingDecisionDestinationResponse
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";
}