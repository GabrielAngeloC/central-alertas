namespace CentralAlertas.Api.Contracts.Sources;

public class SourceHealthResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ExpectedIntervalMinutes { get; set; }

    public DateTime? LastReceivedAt { get; set; }

    public DateTime? NextExpectedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public int MinutesLate { get; set; }

    public bool IsSilent { get; set; }

    public bool IsActive { get; set; }
}