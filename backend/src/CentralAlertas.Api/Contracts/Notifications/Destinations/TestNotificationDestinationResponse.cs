namespace CentralAlertas.Api.Contracts.Notifications.Destinations;

public class TestNotificationDestinationResponse
{
    public Guid DestinationId { get; set; }

    public string DestinationName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? Error { get; set; }
}