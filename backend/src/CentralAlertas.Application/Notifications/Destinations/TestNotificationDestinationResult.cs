namespace CentralAlertas.Application.Notifications.Destinations;

public class TestNotificationDestinationResult
{
    public Guid DestinationId { get; set; }

    public string DestinationName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Error { get; set; }

    public bool WasNotFound { get; set; }
}