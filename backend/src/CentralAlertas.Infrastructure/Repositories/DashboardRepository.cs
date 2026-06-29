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
        // Visões configuradas vêm do banco (administráveis pela interface).
        var configuredViews = await _dbContext.DashboardViews
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

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

    public async Task<DashboardStatistics> GetStatisticsAsync(
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var cutoff = today.AddDays(-29); // janela de 30 dias (inclusivo)

        var perDayRaw = await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.FirstSeenAt >= cutoff)
            .GroupBy(x => x.FirstSeenAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var byCategory = await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.LastSeenAt >= cutoff)
            .GroupBy(x => x.Category)
            .Select(g => new DashboardLabelCount { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        var byType = await _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.LastSeenAt >= cutoff)
            .GroupBy(x => x.Type)
            .Select(g => new DashboardLabelCount { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);

        // Preenche os dias sem alertas com zero, para a série ficar contínua.
        var perDay = new List<DashboardDailyCount>();
        for (var day = cutoff; day <= today; day = day.AddDays(1))
        {
            var match = perDayRaw.FirstOrDefault(x => x.Date == day);
            perDay.Add(new DashboardDailyCount { Date = day, Count = match?.Count ?? 0 });
        }

        return new DashboardStatistics
        {
            AlertsPerDay = perDay,
            ByCategory = byCategory,
            ByType = byType
        };
    }

    public async Task<DashboardHubHealth> GetHubHealthAsync(
        CancellationToken cancellationToken)
    {
        const int windowHours = 24;
        var cutoff = DateTime.UtcNow.AddHours(-windowHours);

        var rows = await _dbContext.AlertDeliveries
            .AsNoTracking()
            .Where(x => x.AttemptedAt >= cutoff)
            .GroupBy(x => new { x.Channel, x.Status })
            .Select(g => new { g.Key.Channel, g.Key.Status, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int Count(string? channel, string status) =>
            rows
                .Where(r => (channel == null || r.Channel == channel) && r.Status == status)
                .Sum(r => r.Count);

        var channels = rows
            .Select(r => r.Channel)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        return new DashboardHubHealth
        {
            WindowHours = windowHours,
            TotalDeliveries = rows.Sum(r => r.Count),
            SuccessCount = Count(null, "success"),
            FailedCount = Count(null, "failed"),
            SkippedCount = Count(null, "skipped"),
            ByChannel = channels
                .Select(c => new DashboardChannelHealth
                {
                    Channel = c,
                    Success = Count(c, "success"),
                    Failed = Count(c, "failed"),
                    Skipped = Count(c, "skipped")
                })
                .ToList()
        };
    }

}