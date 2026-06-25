using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Sources;

public class SourceResult
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ExpectedIntervalMinutes { get; set; }

    public DateTime? LastReceivedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public static SourceResult FromEntity(Source source)
    {
        return new SourceResult
        {
            Id = source.Id,
            Name = source.Name,
            ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
            LastReceivedAt = source.LastReceivedAt,
            IsActive = source.IsActive,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt
        };
    }
}