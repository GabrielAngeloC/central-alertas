using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Destinations;

public class NotificationDestinationResult
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string ConfigurationJson { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public static NotificationDestinationResult FromEntity(
        NotificationDestination destination)
    {
        return new NotificationDestinationResult
        {
            Id = destination.Id,
            Name = destination.Name,
            Type = destination.Type,
            ConfigurationJson = destination.ConfigurationJson,
            IsActive = destination.IsActive,
            CreatedAt = destination.CreatedAt,
            UpdatedAt = destination.UpdatedAt
        };
    }
}