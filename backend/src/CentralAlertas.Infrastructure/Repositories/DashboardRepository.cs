using CentralAlertas.Application.Dashboard;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public DashboardRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardSummary> GetSummaryAsync(
        CancellationToken cancellationToken)
    {
        var activeAlertsQuery = _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.IsActive);

        var activeCriticalCount = await activeAlertsQuery
            .CountAsync(x => x.Severity == "critical", cancellationToken);

        var activeWarningCount = await activeAlertsQuery
            .CountAsync(x => x.Severity == "warning", cancellationToken);

        var activeInfoCount = await activeAlertsQuery
            .CountAsync(x => x.Severity == "info", cancellationToken);

        var activeAlertsCount = await activeAlertsQuery
            .CountAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var activeSources = await _dbContext.Sources
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        var totalSourcesCount = activeSources.Count;

        var silentSourcesCount = activeSources.Count(source =>
        {
            if (source.LastReceivedAt is null)
                return true;

            var nextExpectedAt = source.LastReceivedAt.Value
                .AddMinutes(source.ExpectedIntervalMinutes);

            return nextExpectedAt < now;
        });

        var healthySourcesCount = totalSourcesCount - silentSourcesCount;

        var alertsBySeverity = await activeAlertsQuery
            .GroupBy(x => x.Severity)
            .Select(group => new DashboardSeverityCount
            {
                Severity = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(x => x.Severity == "critical")
            .ThenByDescending(x => x.Severity == "warning")
            .ThenByDescending(x => x.Severity == "info")
            .ToListAsync(cancellationToken);

        var alertsByCategory = await activeAlertsQuery
            .GroupBy(x => x.Category)
            .Select(group => new DashboardCategoryCount
            {
                Category = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Category)
            .ToListAsync(cancellationToken);

        var latestAlerts = await activeAlertsQuery
            .OrderByDescending(x => x.LastSeenAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new DashboardSummary
        {
            ActiveCriticalCount = activeCriticalCount,
            ActiveWarningCount = activeWarningCount,
            ActiveInfoCount = activeInfoCount,
            ActiveAlertsCount = activeAlertsCount,
            AlertsBySeverity = alertsBySeverity,
            AlertsByCategory = alertsByCategory,
            LatestAlerts = latestAlerts,
            HealthySourcesCount = healthySourcesCount,
            SilentSourcesCount = silentSourcesCount,
            TotalSourcesCount = totalSourcesCount
        };
    }

    public async Task<List<DashboardView>> GetViewsAsync(
        CancellationToken cancellationToken)
    {
        var configuredViews = GetInitialConfiguredViews();

        var configuredCategories = configuredViews
            .Select(x => x.Category)
            .ToList();

        var activeAlerts = await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.Severity == "critical")
            .ThenByDescending(x => x.Severity == "warning")
            .ThenByDescending(x => x.LastSeenAt)
            .ToListAsync(cancellationToken);

        var views = configuredViews
            .Select(view => new DashboardView
            {
                Category = view.Category,
                Title = view.Title,
                Order = view.Order,
                Alerts = activeAlerts
                    .Where(alert => alert.Category == view.Category)
                    .ToList()
            })
            .OrderBy(x => x.Order)
            .ToList();

        var unconfiguredAlerts = activeAlerts
            .Where(alert => !configuredCategories.Contains(alert.Category))
            .ToList();

        if (unconfiguredAlerts.Count > 0)
        {
            views.Add(new DashboardView
            {
                Category = "outros",
                Title = "Outros",
                Order = 999,
                Alerts = unconfiguredAlerts
            });
        }

        return views;
    }
    private static List<DashboardViewConfiguration> GetInitialConfiguredViews()
    {
        return
        [
            new DashboardViewConfiguration
            {
                Category = "pedidos",
                Title = "Pedidos & Notas",
                Order = 1
            },
            new DashboardViewConfiguration
            {
                Category = "faturamento",
                Title = "Faturamento",
                Order = 2
            },
            new DashboardViewConfiguration
            {
                Category = "cotacao",
                Title = "Cotação de frete",
                Order = 3
            },
            new DashboardViewConfiguration
            {
                Category = "infraestrutura",
                Title = "Internet & Infra",
                Order = 4
            },
            new DashboardViewConfiguration
            {
                Category = "estoque",
                Title = "Estoque",
                Order = 5
            }
        ];
    }

    private class DashboardViewConfiguration
    {
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}