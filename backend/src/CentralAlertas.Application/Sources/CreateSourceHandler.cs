using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Sources;

public class CreateSourceHandler
{
    private readonly ISourceRepository _sourceRepository;

    public CreateSourceHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<CreateSourceResult> HandleAsync(
        string name,
        int expectedIntervalMinutes,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CreateSourceResult.Failure("Nome da source é obrigatório.");

        if (expectedIntervalMinutes <= 0)
            return CreateSourceResult.Failure("Intervalo esperado deve ser maior que zero.");

        var normalizedName = name.Trim();

        var existing = await _sourceRepository.GetByNameAsync(
            normalizedName,
            cancellationToken);

        if (existing is not null)
            return CreateSourceResult.Failure("Já existe uma source com esse nome.");

        var source = new Source(
            normalizedName,
            expectedIntervalMinutes);

        await _sourceRepository.AddAsync(source, cancellationToken);
        await _sourceRepository.SaveChangesAsync(cancellationToken);

        return CreateSourceResult.Success(SourceResult.FromEntity(source));
    }
}

public class CreateSourceResult
{
    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public SourceResult? Source { get; set; }

    public static CreateSourceResult Success(SourceResult source)
    {
        return new CreateSourceResult
        {
            IsSuccess = true,
            Source = source
        };
    }

    public static CreateSourceResult Failure(string errorMessage)
    {
        return new CreateSourceResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}