using CentralAlertas.Application.Alerts;

namespace CentralAlertas.Application.Sources;

public class ResolveSilentSourceAlertHandler
{
    private readonly IAlertRepository _alertRepository;

    public ResolveSilentSourceAlertHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task HandleAsync(
        string sourceName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
            return;

        if (sourceName == "central-alertas")
            return;

        var dedupKey = $"fonte_silenciosa:{sourceName}";

        var alert = await _alertRepository.GetBySourceAndDedupKeyAsync(
            source: "central-alertas",
            dedupKey: dedupKey,
            cancellationToken: cancellationToken);

        if (alert is null)
            return;

        if (!alert.IsActive)
            return;

        alert.Resolve("Fonte voltou a enviar dados.");

        await _alertRepository.SaveChangesAsync(cancellationToken);
    }
}