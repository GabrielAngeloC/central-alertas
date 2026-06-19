using CentralAlertas.Application.Alerts;
using CentralAlertas.Application.Dashboard;
using CentralAlertas.Infrastructure.Persistence;
using CentralAlertas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CentralAlertas.Application.Sources;

namespace CentralAlertas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CentralAlertasDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ISourceRepository, SourceRepository>();

        return services;
    }
}