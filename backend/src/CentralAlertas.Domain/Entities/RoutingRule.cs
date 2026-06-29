namespace CentralAlertas.Domain.Entities;

public class RoutingRule
{
    private readonly List<RoutingRuleDestination> _destinations = [];

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public int Order { get; private set; }

    public string? Severity { get; private set; }

    public string? Category { get; private set; }

    public string? Type { get; private set; }

    public string? Source { get; private set; }

    public string DeliveryMode { get; private set; } = "immediate";

    public int? ThrottleMinutes { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<RoutingRuleDestination> Destinations =>
        _destinations.AsReadOnly();

    private RoutingRule()
    {
    }

    public RoutingRule(
        string name,
        int order,
        string? severity,
        string? category,
        string? type,
        string? source,
        string deliveryMode,
        int? throttleMinutes)
    {
        Name = name;
        Order = order;
        Severity = NormalizeNullable(severity);
        Category = NormalizeNullable(category);
        Type = NormalizeNullable(type);
        Source = NormalizeNullable(source);
        DeliveryMode = deliveryMode;
        ThrottleMinutes = throttleMinutes;
    }

    public void Update(
        string name,
        int order,
        string? severity,
        string? category,
        string? type,
        string? source,
        string deliveryMode,
        int? throttleMinutes,
        bool isActive)
    {
        Name = name;
        Order = order;
        Severity = NormalizeNullable(severity);
        Category = NormalizeNullable(category);
        Type = NormalizeNullable(type);
        Source = NormalizeNullable(source);
        DeliveryMode = deliveryMode;
        ThrottleMinutes = throttleMinutes;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReplaceDestinations(IEnumerable<Guid> destinationIds)
    {
        _destinations.Clear();

        foreach (var destinationId in destinationIds.Distinct())
        {
            _destinations.Add(new RoutingRuleDestination(Id, destinationId));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool Matches(
        string severity,
        string category,
        string type,
        string source)
    {
        if (!string.IsNullOrWhiteSpace(Severity) && Severity != severity)
            return false;

        if (!string.IsNullOrWhiteSpace(Category) && Category != category)
            return false;

        if (!string.IsNullOrWhiteSpace(Type) && Type != type)
            return false;

        if (!string.IsNullOrWhiteSpace(Source) && Source != source)
            return false;

        return true;
    }

    private static string? NormalizeNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}