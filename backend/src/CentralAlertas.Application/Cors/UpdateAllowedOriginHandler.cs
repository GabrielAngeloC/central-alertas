using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Cors;

public class UpdateAllowedOriginHandler
{
    private readonly IAllowedOriginRepository _repository;
    private readonly IAllowedOriginsCache _cache;

    public UpdateAllowedOriginHandler(
        IAllowedOriginRepository repository,
        IAllowedOriginsCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<AllowedOriginCommandResult> HandleAsync(
        Guid id,
        string? origin,
        string? description,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return AllowedOriginCommandResult.NotFound();

        if (!OriginValidator.IsValid(origin, out var error))
            return AllowedOriginCommandResult.Invalid(error!);

        var normalized = AllowedOrigin.Normalize(origin!);

        var duplicate = await _repository.GetByOriginAsync(normalized, cancellationToken);
        if (duplicate is not null && duplicate.Id != id)
            return AllowedOriginCommandResult.Invalid("Já existe outra origem cadastrada com esse valor.");

        entity.Update(normalized, description, isActive);

        await _repository.SaveChangesAsync(cancellationToken);
        await _cache.RefreshAsync(cancellationToken);

        return AllowedOriginCommandResult.Success(AllowedOriginResult.FromEntity(entity));
    }
}
