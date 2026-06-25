using CentralAlertas.Application.Sources;
using Microsoft.Extensions.Options;

namespace CentralAlertas.Api.Workers.Heartbeat;

public class HeartbeatWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<HeartbeatWorker> _logger;
    private readonly HeartbeatWorkerOptions _options;

    public HeartbeatWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<HeartbeatWorker> logger,
        IOptions<HeartbeatWorkerOptions> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("HeartbeatWorker está desativado por configuração.");
            return;
        }

        var intervalSeconds = Math.Max(_options.IntervalSeconds, 10);

        _logger.LogInformation(
            "HeartbeatWorker iniciado. Intervalo: {IntervalSeconds} segundos.",
            intervalSeconds);

        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(intervalSeconds));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCheckAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Encerramento normal da aplicação.
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao executar verificação automática de heartbeat.");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("HeartbeatWorker finalizado.");
    }

    private async Task RunCheckAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService<CheckSilentSourcesHandler>();

        var result = await handler.HandleAsync(cancellationToken);

        if (result.SilentSourcesCount == 0)
        {
            _logger.LogInformation(
                "Heartbeat verificado. Fontes checadas: {CheckedSourcesCount}. Nenhuma fonte silenciosa.",
                result.CheckedSourcesCount);

            return;
        }

        _logger.LogWarning(
            "Heartbeat detectou {SilentSourcesCount} fonte(s) silenciosa(s) de {CheckedSourcesCount} checada(s).",
            result.SilentSourcesCount,
            result.CheckedSourcesCount);

        foreach (var alert in result.Alerts)
        {
            _logger.LogWarning(
                "Fonte silenciosa: {SourceName}. Status: {Status}. Atraso: {MinutesLate} min. Alerta: {AlertId}. Ocorrências: {OccurrenceCount}.",
                alert.SourceName,
                alert.Status,
                alert.MinutesLate,
                alert.AlertId,
                alert.OccurrenceCount);
        }
    }
}