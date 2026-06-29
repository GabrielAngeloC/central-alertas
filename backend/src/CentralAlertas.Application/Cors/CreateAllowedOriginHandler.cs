using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Cors;

public class CreateAllowedOriginHandler
{
    private readonly IAllowedOriginRepository _repository;
    private readonly IAllowedOriginsCache _cache;

    public CreateAllowedOriginHandler(
        IAllowedOriginRepository repository,
        IAllowedOriginsCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<AllowedOriginCommandResult> HandleAsync(
        string? origin,
        string? description,
        CancellationToken cancellationToken)
    {
        if (!OriginValidator.IsValid(origin, out var error))
            return AllowedOriginCommandResult.Invalid(error!);

        var normalized = AllowedOrigin.Normalize(origin!);

        var existing = await _repository.GetByOriginAsync(normalized, cancellationToken);
        if (existing is not null)
            return AllowedOriginCommandResult.Invalid("Já existe uma origem cadastrada com esse valor.");

        var entity = new AllowedOrigin(normalized, description);

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        await _cache.RefreshAsync(cancellationToken);

        return AllowedOriginCommandResult.Success(AllowedOriginResult.FromEntity(entity));
    }
}

public class AllowedOriginCommandResult
{
    public bool IsSuccess { get; set; }

    public bool WasNotFound { get; set; }

    public string? ErrorMessage { get; set; }

    public AllowedOriginResult? Origin { get; set; }

    public static AllowedOriginCommandResult Success(AllowedOriginResult origin) =>
        new() { IsSuccess = true, Origin = origin };

    public static AllowedOriginCommandResult Invalid(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

    public static AllowedOriginCommandResult NotFound() =>
        new() { IsSuccess = false, WasNotFound = true, ErrorMessage = "Origem não encontrada." };
}
