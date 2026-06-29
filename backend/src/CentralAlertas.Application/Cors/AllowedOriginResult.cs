using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Cors;

public class AllowedOriginResult
{
    public Guid Id { get; set; }

    public string Origin { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public static AllowedOriginResult FromEntity(AllowedOrigin origin)
    {
        return new AllowedOriginResult
        {
            Id = origin.Id,
            Origin = origin.Origin,
            Description = origin.Description,
            IsActive = origin.IsActive,
            CreatedAt = origin.CreatedAt,
            UpdatedAt = origin.UpdatedAt
        };
    }
}
