using CentralAlertas.Application.Authentication;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public AppUserRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AppUser?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return _dbContext.AppUsers
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken)
    {
        return _dbContext.AppUsers.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        await _dbContext.AppUsers.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}