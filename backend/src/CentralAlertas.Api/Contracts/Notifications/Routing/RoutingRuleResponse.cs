namespace CentralAlertas.Api.Contracts.Notifications.Routing;

public class RoutingRuleResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Order { get; set; }

    public string? Severity { get; set; }

    public string? Category { get; set; }

    public string? Type { get; set; }

    public string? Source { get; set; }

    public string DeliveryMode { get; set; } = string.Empty;

    public int? ThrottleMinutes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<RoutingRuleDestinationResponse> Destinations { get; set; } = [];
}

public class RoutingRuleDestinationResponse
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}