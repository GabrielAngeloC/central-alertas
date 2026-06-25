namespace CentralAlertas.Domain.Entities;

public class NotificationDestination
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public string ConfigurationJson { get; private set; } = "{}";

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    private NotificationDestination()
    {
    }

    public NotificationDestination(
        string name,
        string type,
        string configurationJson)
    {
        Name = name;
        Type = type;
        ConfigurationJson = configurationJson;
    }

    public void Update(
        string name,
        string type,
        string configurationJson,
        bool isActive)
    {
        Name = name;
        Type = type;
        ConfigurationJson = configurationJson;
        IsActive = isActive;
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
}