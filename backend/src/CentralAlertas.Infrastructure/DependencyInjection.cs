using CentralAlertas.Application.Alerts;
using CentralAlertas.Application.Dashboard;
using CentralAlertas.Infrastructure.Persistence;
using CentralAlertas.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CentralAlertas.Application.Sources;
using CentralAlertas.Application.Notifications.Destinations;
using CentralAlertas.Application.Notifications.Routing;
using CentralAlertas.Application.Notifications.Deliveries;
using CentralAlertas.Application.Notifications.Dispatching;
using CentralAlertas.Infrastructure.Notifications.Telegram;
using CentralAlertas.Infrastructure.Notifications.Email;
using CentralAlertas.Application.Authentication;
using CentralAlertas.Infrastructure.Authentication;


namespace CentralAlertas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,   IConfiguration configuration)
    {

        services.AddDbContext<CentralAlertasDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ISourceRepository, SourceRepository>();
        services.AddScoped<INotificationDestinationRepository, NotificationDestinationRepository>();
        services.AddScoped<IRoutingRuleRepository, RoutingRuleRepository>();

        services.AddScoped<GetNotificationDestinationsHandler>();
        services.AddScoped<CreateNotificationDestinationHandler>();
        services.AddScoped<UpdateNotificationDestinationHandler>();

        services.AddScoped<GetRoutingRulesHandler>();
        services.AddScoped<CreateRoutingRuleHandler>();
        services.AddScoped<UpdateRoutingRuleHandler>();

        services.AddScoped<IAppUserRepository, AppUserRepository>();

        services.AddScoped<IAlertDeliveryRepository, AlertDeliveryRepository>();

        services.AddScoped<AdminUserSeeder>();

        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));

        services.AddHttpClient<TelegramNotificationChannel>(client =>
        {
            client.BaseAddress = new Uri("https://api.telegram.org/");
        });

        services.AddScoped<INotificationChannel>(provider =>
        provider.GetRequiredService<TelegramNotificationChannel>());

        services.Configure<EmailOptions>(
        configuration.GetSection("Email"));

        services.AddScoped<EmailNotificationChannel>();

        services.AddScoped<INotificationChannel>(provider =>
        provider.GetRequiredService<EmailNotificationChannel>());



        return services;
    }

}