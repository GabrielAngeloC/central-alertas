namespace CentralAlertas.Application.Notifications.Routing;

public class RoutingDecision
{
    public bool Matched { get; set; }

    public Guid? RuleId { get; set; }

    public string? RuleName { get; set; }

    public string DeliveryMode { get; set; } = "store_only";

    public int? ThrottleMinutes { get; set; }

    public List<RoutingDecisionDestination> Destinations { get; set; } = [];

    public static RoutingDecision NoMatch()
    {
        return new RoutingDecision
        {
            Matched = false,
            DeliveryMode = "store_only",
            Destinations = []
        };
    }
}

public class RoutingDecisionDestination
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";
}