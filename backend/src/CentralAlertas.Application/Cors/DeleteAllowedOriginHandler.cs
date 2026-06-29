namespace CentralAlertas.Application.Cors;

public class DeleteAllowedOriginHandler
{
    private readonly IAllowedOriginRepository _repository;
    private readonly IAllowedOriginsCache _cache;

    public DeleteAllowedOriginHandler(
        IAllowedOriginRepository repository,
        IAllowedOriginsCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return false;

        entity.Delete();
        await _repository.SaveChangesAsync(cancellationToken);
        await _cache.RefreshAsync(cancellationToken);

        return true;
    }
}
