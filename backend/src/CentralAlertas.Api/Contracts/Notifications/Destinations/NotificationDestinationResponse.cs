namespace CentralAlertas.Api.Contracts.Notifications.Destinations;

public class NotificationDestinationResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}