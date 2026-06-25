using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Authentication;

public interface IAppUserRepository
{
    Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<bool> AnyAsync(CancellationToken cancellationToken);

    Task AddAsync(
        AppUser user,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}