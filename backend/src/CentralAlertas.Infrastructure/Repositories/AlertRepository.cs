using CentralAlertas.Application.Alerts;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly CentralAlertasDbContext _dbContext;

    public AlertRepository(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Alert?> GetBySourceAndDedupKeyAsync(
        string source,
        string dedupKey,
        CancellationToken cancellationToken)
    {
        return _dbContext.Alerts
            .FirstOrDefaultAsync(
                x => x.Source == source && x.DedupKey == dedupKey,
                cancellationToken);
    }

    public Task<Alert?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return _dbContext.Alerts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Alert>> SearchByItemAsync(
        string term,
        CancellationToken cancellationToken)
    {
        // Procura o termo em items/payload (jsonb convertido para texto),
        // título e dedup_key. ILIKE = case-insensitive no PostgreSQL.
        var pattern = $"%{term}%";

        return _dbContext.Alerts
            .FromSqlInterpolated($@"
                SELECT * FROM alerts
                WHERE ""ItemsJson""::text ILIKE {pattern}
                   OR ""PayloadJson""::text ILIKE {pattern}
                   OR ""Title"" ILIKE {pattern}
                   OR ""DedupKey"" ILIKE {pattern}
                ORDER BY ""LastSeenAt"" DESC
                LIMIT 200")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<List<Alert>> GetActiveAsync(
        string? severity,
        string? category,
        string? source,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Alerts
            .AsNoTracking()
            .Where(x => x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(severity))
        {
            var normalizedSeverity = severity.Trim().ToLower();
            query = query.Where(x => x.Severity == normalizedSeverity);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            query = query.Where(x => x.Category == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            var normalizedSource = source.Trim();
            query = query.Where(x => x.Source == normalizedSource);
        }

        return query
            .OrderByDescending(x => x.Severity == "critical")
            .ThenByDescending(x => x.Severity == "warning")
            .ThenByDescending(x => x.LastSeenAt)
            .ToListAsync(cancellationToken);
    }

    public Task<List<AlertOccurrence>> GetOccurrencesAsync(
        Guid alertId,
        CancellationToken cancellationToken)
    {
        return _dbContext.AlertOccurrences
            .AsNoTracking()
            .Where(x => x.AlertId == alertId)
            .OrderByDescending(x => x.ReceivedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Alert alert,
        CancellationToken cancellationToken)
    {
        await _dbContext.Alerts.AddAsync(alert, cancellationToken);
    }
    public Task<List<Alert>> GetResolvedAsync(
    string? severity,
    string? category,
    string? source,
    CancellationToken cancellationToken)
    {
        var query = _dbContext.Alerts
            .AsNoTracking()
            .Where(x => !x.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(severity))
        {
            var normalizedSeverity = severity.Trim().ToLower();
            query = query.Where(x => x.Severity == normalizedSeverity);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            query = query.Where(x => x.Category == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            var normalizedSource = source.Trim();
            query = query.Where(x => x.Source == normalizedSource);
        }

        return query
            .OrderByDescending(x => x.ResolvedAt)
            .ThenByDescending(x => x.LastSeenAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddOccurrenceAsync(
        AlertOccurrence occurrence,
        CancellationToken cancellationToken)
    {
        await _dbContext.AlertOccurrences.AddAsync(occurrence, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Alert>> GetAsync(
    string status,
    string? severity,
    string? category,
    string? source,
    DateTime? from,
    DateTime? to,
    CancellationToken cancellationToken)
    {
        var query = _dbContext.Alerts
            .AsNoTracking()
            .AsQueryable();

        if (status == "active")
        {
            query = query.Where(x => x.IsActive);
        }
        else if (status == "resolved")
        {
            query = query.Where(x => !x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            var normalizedSeverity = severity.Trim().ToLower();
            query = query.Where(x => x.Severity == normalizedSeverity);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var normalizedCategory = category.Trim();
            query = query.Where(x => x.Category == normalizedCategory);
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            var normalizedSource = source.Trim();
            query = query.Where(x => x.Source == normalizedSource);
        }

        if (from is not null)
        {
            query = query.Where(x => x.LastSeenAt >= from.Value);
        }

        if (to is not null)
        {
            query = query.Where(x => x.LastSeenAt <= to.Value);
        }

        return query
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.LastSeenAt)
            .ToListAsync(cancellationToken);
    }
}