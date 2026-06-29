namespace CentralAlertas.Application.Cors;

public class GetAllowedOriginsHandler
{
    private readonly IAllowedOriginRepository _repository;

    public GetAllowedOriginsHandler(IAllowedOriginRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AllowedOriginResult>> HandleAsync(CancellationToken cancellationToken)
    {
        var origins = await _repository.GetAllAsync(cancellationToken);

        return origins
            .Select(AllowedOriginResult.FromEntity)
            .ToList();
    }
}
