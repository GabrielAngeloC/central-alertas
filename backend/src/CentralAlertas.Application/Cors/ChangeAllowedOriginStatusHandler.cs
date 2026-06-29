namespace CentralAlertas.Application.Cors;

public class ChangeAllowedOriginStatusHandler
{
    private readonly IAllowedOriginRepository _repository;
    private readonly IAllowedOriginsCache _cache;

    public ChangeAllowedOriginStatusHandler(
        IAllowedOriginRepository repository,
        IAllowedOriginsCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public Task<AllowedOriginCommandResult> ActivateAsync(Guid id, CancellationToken cancellationToken) =>
        ChangeAsync(id, activate: true, cancellationToken);

    public Task<AllowedOriginCommandResult> DeactivateAsync(Guid id, CancellationToken cancellationToken) =>
        ChangeAsync(id, activate: false, cancellationToken);

    private async Task<AllowedOriginCommandResult> ChangeAsync(
        Guid id,
        bool activate,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return AllowedOriginCommandResult.NotFound();

        if (activate) entity.Activate();
        else entity.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);
        await _cache.RefreshAsync(cancellationToken);

        return AllowedOriginCommandResult.Success(AllowedOriginResult.FromEntity(entity));
    }
}
