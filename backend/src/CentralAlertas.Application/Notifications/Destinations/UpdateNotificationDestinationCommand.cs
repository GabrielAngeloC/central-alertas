namespace CentralAlertas.Application.Notifications.Destinations;

public class UpdateNotificationDestinationCommand
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";

    public bool IsActive { get; set; }
}