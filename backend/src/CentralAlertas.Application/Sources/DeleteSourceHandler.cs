namespace CentralAlertas.Application.Sources;

public class DeleteSourceHandler
{
    private readonly ISourceRepository _repository;

    public DeleteSourceHandler(ISourceRepository repository)
    {
        _repository = repository;
    }

    // Sources são apenas cadastro (alertas referenciam pelo nome, sem FK),
    // então a exclusão é segura.
    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var source = await _repository.GetByIdAsync(id, cancellationToken);
        if (source is null)
            return false;

        source.Delete();
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
