using CentralAlertas.Application.Authentication;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CentralAlertas.Infrastructure.Authentication;

public class AdminUserSeeder
{
    private readonly CentralAlertasDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher _passwordHasher;

    public AdminUserSeeder(
        CentralAlertasDbContext dbContext,
        IConfiguration configuration,
        PasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.AppUsers
            .AnyAsync(cancellationToken);

        if (exists)
            return;

        var name = _configuration["AdminUser:Name"] ?? "Administrador";
        var email = _configuration["AdminUser:Email"] ?? "admin@centralalertas.local";
        var password = _configuration["AdminUser:Password"] ?? "Admin@123";

        var passwordHash = _passwordHasher.Hash(password);

        var user = new AppUser(
            name.Trim(),
            email.Trim().ToLower(),
            passwordHash);

        await _dbContext.AppUsers.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}