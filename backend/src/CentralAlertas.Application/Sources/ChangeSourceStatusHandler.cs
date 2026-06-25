namespace CentralAlertas.Application.Sources;

public class ChangeSourceStatusHandler
{
    private readonly ISourceRepository _sourceRepository;

    public ChangeSourceStatusHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<ChangeSourceStatusResult> ActivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await _sourceRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (source is null)
            return ChangeSourceStatusResult.NotFound();

        source.Activate();

        await _sourceRepository.SaveChangesAsync(cancellationToken);

        return ChangeSourceStatusResult.Success(SourceResult.FromEntity(source));
    }

    public async Task<ChangeSourceStatusResult> DeactivateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await _sourceRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (source is null)
            return ChangeSourceStatusResult.NotFound();

        source.Deactivate();

        await _sourceRepository.SaveChangesAsync(cancellationToken);

        return ChangeSourceStatusResult.Success(SourceResult.FromEntity(source));
    }
}

public class ChangeSourceStatusResult
{
    public bool IsSuccess { get; set; }

    public bool WasNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public SourceResult? Source { get; set; }

    public static ChangeSourceStatusResult Success(SourceResult source)
    {
        return new ChangeSourceStatusResult
        {
            IsSuccess = true,
            Source = source
        };
    }

    public static ChangeSourceStatusResult NotFound()
    {
        return new ChangeSourceStatusResult
        {
            IsSuccess = false,
            WasNotFound = true,
            ErrorMessage = "Source não encontrada."
        };
    }
}