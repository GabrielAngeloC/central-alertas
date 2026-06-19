namespace CentralAlertas.Application.Sources;

public class GetSourcesHealthHandler
{
    private readonly ISourceRepository _sourceRepository;

    public GetSourcesHealthHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<List<SourceHealth>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var sources = await _sourceRepository.GetAllAsync(cancellationToken);

        return sources
            .Select(source => BuildHealth(source, now))
            .OrderByDescending(x => x.IsSilent)
            .ThenBy(x => x.Status == "healthy")
            .ThenBy(x => x.Name)
            .ToList();
    }

    private static SourceHealth BuildHealth(
        Domain.Entities.Source source,
        DateTime now)
    {
        if (!source.IsActive)
        {
            return new SourceHealth
            {
                Id = source.Id,
                Name = source.Name,
                ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
                LastReceivedAt = source.LastReceivedAt,
                NextExpectedAt = null,
                Status = "inactive",
                MinutesLate = 0,
                IsSilent = false,
                IsActive = source.IsActive
            };
        }

        if (source.LastReceivedAt is null)
        {
            return new SourceHealth
            {
                Id = source.Id,
                Name = source.Name,
                ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
                LastReceivedAt = null,
                NextExpectedAt = null,
                Status = "never_received",
                MinutesLate = 0,
                IsSilent = true,
                IsActive = source.IsActive
            };
        }

        var nextExpectedAt = source.LastReceivedAt.Value
            .AddMinutes(source.ExpectedIntervalMinutes);

        var isSilent = nextExpectedAt < now;

        var minutesLate = isSilent
            ? (int)Math.Floor((now - nextExpectedAt).TotalMinutes)
            : 0;

        return new SourceHealth
        {
            Id = source.Id,
            Name = source.Name,
            ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
            LastReceivedAt = source.LastReceivedAt,
            NextExpectedAt = nextExpectedAt,
            Status = isSilent ? "silent" : "healthy",
            MinutesLate = minutesLate,
            IsSilent = isSilent,
            IsActive = source.IsActive
        };
    }
}