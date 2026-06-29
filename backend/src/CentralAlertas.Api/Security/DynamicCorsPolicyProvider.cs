using CentralAlertas.Application.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CentralAlertas.Api.Security;

// Provedor de política de CORS que decide as origens permitidas dinamicamente,
// consultando o cache (config + origens cadastradas no banco) a cada requisição.
public class DynamicCorsPolicyProvider : ICorsPolicyProvider
{
    private readonly IAllowedOriginsCache _cache;

    public DynamicCorsPolicyProvider(IAllowedOriginsCache cache)
    {
        _cache = cache;
    }

    public Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
    {
        var builder = new CorsPolicyBuilder();

        builder
            .SetIsOriginAllowed(origin => _cache.IsAllowed(origin))
            .AllowAnyHeader()
            .AllowAnyMethod();

        return Task.FromResult<CorsPolicy?>(builder.Build());
    }
}
