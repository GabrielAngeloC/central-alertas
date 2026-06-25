using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Routing;

public class RoutingRuleResult
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

    public List<RoutingRuleDestinationResult> Destinations { get; set; } = [];

    public static RoutingRuleResult FromEntity(RoutingRule rule)
    {
        return new RoutingRuleResult
        {
            Id = rule.Id,
            Name = rule.Name,
            Order = rule.Order,
            Severity = rule.Severity,
            Category = rule.Category,
            Type = rule.Type,
            Source = rule.Source,
            DeliveryMode = rule.DeliveryMode,
            ThrottleMinutes = rule.ThrottleMinutes,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt,
            Destinations = rule.Destinations
                .Select(destination => new RoutingRuleDestinationResult
                {
                    DestinationId = destination.NotificationDestinationId,
                    Name = destination.NotificationDestination?.Name ?? string.Empty,
                    Type = destination.NotificationDestination?.Type ?? string.Empty
                })
                .ToList()
        };
    }
}

public class RoutingRuleDestinationResult
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;
}