namespace CentralAlertas.Api.Contracts.Sources;

public class SourceResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ExpectedIntervalMinutes { get; set; }

    public DateTime? LastReceivedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}