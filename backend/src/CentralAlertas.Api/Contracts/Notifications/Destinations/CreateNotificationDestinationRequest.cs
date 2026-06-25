using System.Text.Json;

namespace CentralAlertas.Api.Contracts.Notifications.Destinations;

public class CreateNotificationDestinationRequest
{
    public string? Name { get; set; }

    public string? Type { get; set; }

    public JsonElement? Configuration { get; set; }
}