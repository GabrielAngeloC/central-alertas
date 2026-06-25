namespace CentralAlertas.Application.Notifications.Destinations;

public class CreateNotificationDestinationCommand
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = "{}";
}