namespace CentralAlertas.Application.Notifications.Dispatching;

public class NotificationDestinationContext
{
    public Guid DestinationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";
}