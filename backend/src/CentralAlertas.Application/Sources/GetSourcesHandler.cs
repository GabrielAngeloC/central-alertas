using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Sources;

public class GetSourcesHandler
{
    private readonly ISourceRepository _sourceRepository;

    public GetSourcesHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public Task<List<Source>> HandleAsync(CancellationToken cancellationToken)
    {
        return _sourceRepository.GetAllAsync(cancellationToken);
    }
}