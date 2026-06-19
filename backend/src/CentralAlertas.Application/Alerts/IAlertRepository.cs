using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts;

public interface IAlertRepository
{
    Task<Alert?> GetBySourceAndDedupKeyAsync(
        string source,
        string dedupKey,
        CancellationToken cancellationToken);

    Task<Alert?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<Alert>> GetActiveAsync(
        string? severity,
        string? category,
        string? source,
        CancellationToken cancellationToken);

    Task<List<AlertOccurrence>> GetOccurrencesAsync(
        Guid alertId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Alert alert,
        CancellationToken cancellationToken);

    Task AddOccurrenceAsync(
        AlertOccurrence occurrence,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}