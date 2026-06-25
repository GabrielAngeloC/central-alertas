using System.Text.Json;
using CentralAlertas.Application.Alerts.Ingestion;

namespace CentralAlertas.Application.Sources;

public class CheckSilentSourcesHandler
{
    private readonly GetSourcesHealthHandler _getSourcesHealthHandler;
    private readonly CreateAlertHandler _createAlertHandler;

    public CheckSilentSourcesHandler(
        GetSourcesHealthHandler getSourcesHealthHandler,
        CreateAlertHandler createAlertHandler)
    {
        _getSourcesHealthHandler = getSourcesHealthHandler;
        _createAlertHandler = createAlertHandler;
    }

    public async Task<CheckSilentSourcesResult> HandleAsync(
        CancellationToken cancellationToken)
    {
        var sourcesHealth = await _getSourcesHealthHandler.HandleAsync(
            cancellationToken);

        var silentSources = sourcesHealth
            .Where(source => source.IsSilent && source.IsActive)
            .ToList();

        var createdOrUpdatedAlerts = new List<CheckSilentSourceAlertResult>();

        foreach (var source in silentSources)
        {
            var command = BuildAlertCommand(source);

            var result = await _createAlertHandler.HandleAsync(
                command,
                cancellationToken);

            createdOrUpdatedAlerts.Add(new CheckSilentSourceAlertResult
            {
                SourceName = source.Name,
                Status = source.Status,
                MinutesLate = source.MinutesLate,
                AlertId = result.AlertId,
                AlertStatus = result.Status,
                OccurrenceCount = result.OccurrenceCount
            });
        }

        return new CheckSilentSourcesResult
        {
            CheckedSourcesCount = sourcesHealth.Count,
            SilentSourcesCount = silentSources.Count,
            Alerts = createdOrUpdatedAlerts
        };
    }

    private static CreateAlertCommand BuildAlertCommand(SourceHealth source)
    {
        var metricValue = source.MinutesLate;

        if (source.Status == "never_received")
        {
            metricValue = source.ExpectedIntervalMinutes;
        }

        var items = new[]
        {
            new
            {
                source = source.Name,
                status = source.Status,
                expected_interval_minutes = source.ExpectedIntervalMinutes,
                last_received_at = source.LastReceivedAt,
                next_expected_at = source.NextExpectedAt,
                minutes_late = source.MinutesLate
            }
        };

        var payload = new
        {
            detected_by = "central-alertas",
            detection_type = "heartbeat",
            detected_at = DateTime.UtcNow
        };

        return new CreateAlertCommand
        {
            Source = "central-alertas",
            Category = "infraestrutura",
            Type = "fonte_silenciosa",
            Severity = "critical",
            Title = $"Fonte silenciosa: {source.Name}",
            Message = BuildMessage(source),
            DedupKey = $"fonte_silenciosa:{source.Name}",

            MetricValue = metricValue,
            MetricUnit = "minutos",
            MetricThreshold = source.ExpectedIntervalMinutes,

            ItemsJson = JsonSerializer.Serialize(items),
            PayloadJson = JsonSerializer.Serialize(payload)
        };
    }

    private static string BuildMessage(SourceHealth source)
    {
        if (source.Status == "never_received")
        {
            return $"A fonte {source.Name} está cadastrada, mas nunca enviou alertas.";
        }

        return $"A fonte {source.Name} está silenciosa há {source.MinutesLate} minuto(s) além do esperado.";
    }
}

public class CheckSilentSourcesResult
{
    public int CheckedSourcesCount { get; set; }
    public int SilentSourcesCount { get; set; }

    public List<CheckSilentSourceAlertResult> Alerts { get; set; } = [];
}

public class CheckSilentSourceAlertResult
{
    public string SourceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MinutesLate { get; set; }

    public Guid AlertId { get; set; }
    public string AlertStatus { get; set; } = string.Empty;
    public int OccurrenceCount { get; set; }
}