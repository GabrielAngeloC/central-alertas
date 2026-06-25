using System.Text.Json;

namespace CentralAlertas.Api.Contracts.Notifications.Destinations;

public class UpdateNotificationDestinationRequest
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public JsonElement? Configuration { get; set; }

    public bool IsActive { get; set; } = true;
}