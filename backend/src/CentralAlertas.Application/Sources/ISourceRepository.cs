using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Sources;

public interface ISourceRepository
{
    Task<Source?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task<List<Source>> GetAllAsync(
        CancellationToken cancellationToken);

    Task AddAsync(
        Source source,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}