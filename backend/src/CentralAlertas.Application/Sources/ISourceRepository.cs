using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Sources;

public interface ISourceRepository
{
    Task<List<Source>> GetAllAsync(CancellationToken cancellationToken);

    Task<Source?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Source?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task AddAsync(
        Source source,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}