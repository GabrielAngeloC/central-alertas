namespace CentralAlertas.Application.Cors;

public class GetAllowedOriginByIdHandler
{
    private readonly IAllowedOriginRepository _repository;

    public GetAllowedOriginByIdHandler(IAllowedOriginRepository repository)
    {
        _repository = repository;
    }

    public async Task<AllowedOriginResult?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var origin = await _repository.GetByIdAsync(id, cancellationToken);

        return origin is null ? null : AllowedOriginResult.FromEntity(origin);
    }
}
