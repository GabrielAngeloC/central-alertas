using CentralAlertas.Application.Alerts.Ingestion;
using CentralAlertas.Application.Alerts.Queries;
using CentralAlertas.Application.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using CentralAlertas.Application.Sources;

namespace CentralAlertas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateAlertHandler>();

        services.AddScoped<GetActiveAlertsHandler>();
        services.AddScoped<GetAlertByIdHandler>();
        services.AddScoped<GetAlertOccurrencesHandler>();

        services.AddScoped<GetDashboardSummaryHandler>();
        services.AddScoped<GetDashboardViewsHandler>();

        services.AddScoped<GetSourcesHandler>();
        services.AddScoped<GetSourcesHealthHandler>();

        return services;
    }
}