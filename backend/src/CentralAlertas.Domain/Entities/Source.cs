namespace CentralAlertas.Domain.Entities;

public class Source
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public int ExpectedIntervalMinutes { get; private set; }

    public DateTime? LastReceivedAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    private Source()
    {
    }

    public Source(string name, int expectedIntervalMinutes)
    {
        Name = name;
        ExpectedIntervalMinutes = expectedIntervalMinutes;
    }

    public void RegisterReception(DateTime receivedAt)
    {
        LastReceivedAt = receivedAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, int expectedIntervalMinutes)
    {
        Name = name;
        ExpectedIntervalMinutes = expectedIntervalMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}