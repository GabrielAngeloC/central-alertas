using CentralAlertas.Application.Alerts;
using CentralAlertas.Application.Sources;
using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Ingestion;

public class CreateAlertHandler
{
    private readonly IAlertRepository _alertRepository;
    private readonly ISourceRepository _sourceRepository;

    public CreateAlertHandler(
        IAlertRepository alertRepository,
        ISourceRepository sourceRepository)
    {
        _alertRepository = alertRepository;
        _sourceRepository = sourceRepository;
    }

    public async Task<CreateAlertResult> HandleAsync(
        CreateAlertCommand command,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        await RegisterSourceReceptionAsync(
            command.Source,
            now,
            cancellationToken);

        var alert = await _alertRepository.GetBySourceAndDedupKeyAsync(
            command.Source,
            command.DedupKey,
            cancellationToken);

        var isNewAlert = false;

        if (alert is null)
        {
            alert = new Alert(
                command.Source,
                command.Category,
                command.Type,
                command.Severity,
                command.Title,
                command.Message,
                command.DedupKey,
                command.MetricValue,
                command.MetricUnit,
                command.MetricThreshold,
                command.ItemsJson,
                command.PayloadJson);

            await _alertRepository.AddAsync(alert, cancellationToken);
            isNewAlert = true;
        }
        else
        {
            alert.RegisterOccurrence(
                command.Severity,
                command.Title,
                command.Message,
                command.MetricValue,
                command.MetricUnit,
                command.MetricThreshold,
                command.ItemsJson,
                command.PayloadJson);
        }

        var occurrence = alert.CreateOccurrence();

        await _alertRepository.AddOccurrenceAsync(occurrence, cancellationToken);

        await _alertRepository.SaveChangesAsync(cancellationToken);

        return new CreateAlertResult
        {
            AlertId = alert.Id,
            Status = isNewAlert ? "created" : "updated",
            OccurrenceCount = alert.OccurrenceCount,
            IsNewAlert = isNewAlert,
            IsEscalating = alert.IsEscalating
        };
    }

    private async Task RegisterSourceReceptionAsync(
        string sourceName,
        DateTime receivedAt,
        CancellationToken cancellationToken)
    {
        var source = await _sourceRepository.GetByNameAsync(
            sourceName,
            cancellationToken);

        if (source is null)
        {
            source = new Source(
                sourceName,
                expectedIntervalMinutes: 60);

            await _sourceRepository.AddAsync(
                source,
                cancellationToken);
        }

        source.RegisterReception(receivedAt);
    }
}