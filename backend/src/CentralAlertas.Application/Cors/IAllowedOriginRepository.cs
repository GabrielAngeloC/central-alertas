using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Cors;

public interface IAllowedOriginRepository
{
    Task<List<AllowedOrigin>> GetAllAsync(CancellationToken cancellationToken);

    Task<AllowedOrigin?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AllowedOrigin?> GetByOriginAsync(string origin, CancellationToken cancellationToken);

    Task<List<string>> GetActiveOriginsAsync(CancellationToken cancellationToken);

    Task AddAsync(AllowedOrigin origin, CancellationToken cancellationToken);

    void Remove(AllowedOrigin origin);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
