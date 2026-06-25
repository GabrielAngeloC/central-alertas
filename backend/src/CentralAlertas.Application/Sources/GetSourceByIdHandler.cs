namespace CentralAlertas.Application.Sources;

public class GetSourceByIdHandler
{
    private readonly ISourceRepository _sourceRepository;

    public GetSourceByIdHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }

    public async Task<SourceResult?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await _sourceRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (source is null)
            return null;

        return SourceResult.FromEntity(source);
    }
}