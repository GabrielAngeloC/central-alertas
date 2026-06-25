namespace CentralAlertas.Application.Sources;

public class UpdateSourceHandler
{
    private readonly ISourceRepository _sourceRepository;

    public UpdateSourceHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<UpdateSourceResult> HandleAsync(
        Guid id,
        string name,
        int expectedIntervalMinutes,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            return UpdateSourceResult.Failure("Nome da source é obrigatório.");

        if (expectedIntervalMinutes <= 0)
            return UpdateSourceResult.Failure("Intervalo esperado deve ser maior que zero.");

        var source = await _sourceRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (source is null)
            return UpdateSourceResult.NotFound();

        var normalizedName = name.Trim();

        var existing = await _sourceRepository.GetByNameAsync(
            normalizedName,
            cancellationToken);

        if (existing is not null && existing.Id != id)
            return UpdateSourceResult.Failure("Já existe outra source com esse nome.");

        source.Update(
            normalizedName,
            expectedIntervalMinutes);

        await _sourceRepository.SaveChangesAsync(cancellationToken);

        return UpdateSourceResult.Success(SourceResult.FromEntity(source));
    }
}

public class UpdateSourceResult
{
    public bool IsSuccess { get; set; }

    public bool WasNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public SourceResult? Source { get; set; }

    public static UpdateSourceResult Success(SourceResult source)
    {
        return new UpdateSourceResult
        {
            IsSuccess = true,
            Source = source
        };
    }

    public static UpdateSourceResult Failure(string errorMessage)
    {
        return new UpdateSourceResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }

    public static UpdateSourceResult NotFound()
    {
        return new UpdateSourceResult
        {
            IsSuccess = false,
            WasNotFound = true,
            ErrorMessage = "Source não encontrada."
        };
    }
}