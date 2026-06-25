using CentralAlertas.Application.Alerts;
using CentralAlertas.Application.Sources;
using CentralAlertas.Domain.Entities;
using CentralAlertas.Application.Notifications.Dispatching;

namespace CentralAlertas.Application.Alerts.Ingestion;

public class CreateAlertHandler
{
    private readonly IAlertRepository _alertRepository;
    private readonly ISourceRepository _sourceRepository;
    private readonly ResolveSilentSourceAlertHandler _resolveSilentSourceAlertHandler;
    private readonly NotificationDispatcher _notificationDispatcher;
    public CreateAlertHandler(
        IAlertRepository alertRepository,
        ISourceRepository sourceRepository,
        ResolveSilentSourceAlertHandler resolveSilentSourceAlertHandler,
        NotificationDispatcher notificationDispatcher)
    {
        _alertRepository = alertRepository;
        _sourceRepository = sourceRepository;
        _resolveSilentSourceAlertHandler = resolveSilentSourceAlertHandler;
        _notificationDispatcher = notificationDispatcher;
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

        await _resolveSilentSourceAlertHandler.HandleAsync(
        sourceName: command.Source,
        cancellationToken: cancellationToken);

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

        await _alertRepository.SaveChangesAsync(cancellationToken);

        await _notificationDispatcher.DispatchAsync(
            alert,
            cancellationToken);

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
                expectedIntervalMinutes: GetDefaultExpectedIntervalMinutes(sourceName));

            await _sourceRepository.AddAsync(
                source,
                cancellationToken);
        }

        source.RegisterReception(receivedAt);
    }
    private static int GetDefaultExpectedIntervalMinutes(string sourceName)
    {
        if (sourceName == "central-alertas")
            return 1440;

        return 60;
    }
}