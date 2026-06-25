using CentralAlertas.Application.Sources;
using CentralAlertas.Application.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using CentralAlertas.Application.Alerts.Queries;
using CentralAlertas.Application.Alerts.Commands;
using CentralAlertas.Application.Alerts.Ingestion;
using CentralAlertas.Application.Notifications.Routing;
using CentralAlertas.Application.Notifications.Dispatching;
using CentralAlertas.Application.Notifications.Deliveries;
using CentralAlertas.Application.Notifications.Destinations;
using CentralAlertas.Application.Authentication;



namespace CentralAlertas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateAlertHandler>();

        services.AddScoped<GetAlertsHandler>();
        services.AddScoped<GetActiveAlertsHandler>();
        services.AddScoped<GetResolvedAlertsHandler>();
        services.AddScoped<GetAlertByIdHandler>();
        services.AddScoped<GetAlertOccurrencesHandler>();
        
        services.AddScoped<ResolveAlertHandler>();

        services.AddScoped<GetDashboardSummaryHandler>();
        services.AddScoped<GetDashboardViewsHandler>();

        services.AddScoped<GetSourcesHandler>();
        services.AddScoped<GetSourcesHealthHandler>();
        services.AddScoped<CheckSilentSourcesHandler>();

        services.AddScoped<GetRoutingRulesHandler>();
        services.AddScoped<CreateRoutingRuleHandler>();
        services.AddScoped<UpdateRoutingRuleHandler>();
        services.AddScoped<RoutingEngine>();

        services.AddScoped<NotificationDispatcher>();
        services.AddScoped<NotificationThrottleService>();

        services.AddScoped<GetAlertDeliveriesHandler>();
        services.AddScoped<TestNotificationDestinationHandler>();
        services.AddScoped<GetNotificationDestinationsHandler>();
        services.AddScoped<CreateNotificationDestinationHandler>();
        services.AddScoped<UpdateNotificationDestinationHandler>();
        services.AddScoped<TestNotificationDestinationHandler>();

        services.AddScoped<ResolveSilentSourceAlertHandler>();
        
        services.AddScoped<PasswordHasher>();
        services.AddScoped<JwtTokenGenerator>();
        services.AddScoped<LoginHandler>();

        services.AddScoped<GetSourceByIdHandler>();
        services.AddScoped<CreateSourceHandler>();
        services.AddScoped<UpdateSourceHandler>();
        services.AddScoped<ChangeSourceStatusHandler>();

        services.AddScoped<GetSourcesHandler>();
        services.AddScoped<GetSourcesHealthHandler>();
        services.AddScoped<CheckSilentSourcesHandler>();

        services.AddScoped<GetNotificationDestinationByIdHandler>();
        services.AddScoped<ChangeNotificationDestinationStatusHandler>();

        return services;
    }
}