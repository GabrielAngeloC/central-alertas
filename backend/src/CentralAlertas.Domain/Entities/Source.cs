namespace CentralAlertas.Domain.Entities;

public class Source
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public int ExpectedIntervalMinutes { get; private set; }

    public DateTime? LastReceivedAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Source()
    {
    }

    public Source(
        string name,
        int expectedIntervalMinutes)
    {
        Name = name;
        ExpectedIntervalMinutes = expectedIntervalMinutes;
    }

    public void RegisterReception(DateTime receivedAt)
    {
        LastReceivedAt = receivedAt;
    }

    public void Update(
        int expectedIntervalMinutes,
        bool isActive)
    {
        ExpectedIntervalMinutes = expectedIntervalMinutes;
        IsActive = isActive;
    }
}