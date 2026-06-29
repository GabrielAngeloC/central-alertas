using CentralAlertas.Application.Cors;
using CentralAlertas.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralAlertas.Infrastructure.Cors;

// Singleton: mantém em memória a união de origens vindas da configuração
// (Cors:AllowedOrigins) com as origens ativas cadastradas no banco.
public class AllowedOriginsCache : IAllowedOriginsCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HashSet<string> _configOrigins;

    private volatile HashSet<string> _dbOrigins =
        new(StringComparer.OrdinalIgnoreCase);

    public AllowedOriginsCache(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;

        var configured = (configuration["Cors:AllowedOrigins"] ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(AllowedOrigin.Normalize);

        _configOrigins = new HashSet<string>(configured, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsAllowed(string origin)
    {
        // Sem nenhuma origem configurada nem cadastrada => libera todas.
        if (_configOrigins.Count == 0 && _dbOrigins.Count == 0)
            return true;

        var normalized = AllowedOrigin.Normalize(origin);
        return _configOrigins.Contains(normalized) || _dbOrigins.Contains(normalized);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAllowedOriginRepository>();

        var origins = await repository.GetActiveOriginsAsync(cancellationToken);

        _dbOrigins = new HashSet<string>(
            origins.Select(AllowedOrigin.Normalize),
            StringComparer.OrdinalIgnoreCase);
    }
}
