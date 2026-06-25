namespace CentralAlertas.Application.Notifications.Routing;

public class RouteAlertCommand
{
    public string Source { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;
}