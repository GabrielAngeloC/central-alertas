using CentralAlertas.Application.Notifications.Dispatching;

namespace CentralAlertas.Application.Notifications.Destinations;

public class TestNotificationDestinationHandler
{
    private readonly INotificationDestinationRepository _destinationRepository;
    private readonly IEnumerable<INotificationChannel> _channels;

    public TestNotificationDestinationHandler(
        INotificationDestinationRepository destinationRepository,
        IEnumerable<INotificationChannel> channels)
    {
        _destinationRepository = destinationRepository;
        _channels = channels;
    }

    public async Task<TestNotificationDestinationResult> HandleAsync(
        Guid destinationId,
        CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(
            destinationId,
            cancellationToken);

        if (destination is null)
        {
            return new TestNotificationDestinationResult
            {
                WasNotFound = true,
                Success = false,
                Message = "Destino não encontrado."
            };
        }

        if (!destination.IsActive)
        {
            return new TestNotificationDestinationResult
            {
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                Type = destination.Type,
                Success = false,
                Message = "Destino está inativo."
            };
        }

        var channel = _channels.FirstOrDefault(x => x.ChannelType == destination.Type);

        if (channel is null)
        {
            return new TestNotificationDestinationResult
            {
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                Type = destination.Type,
                Success = false,
                Message = $"Nenhum canal registrado para o tipo '{destination.Type}'."
            };
        }

        var message = BuildTestMessage();

        try
        {
            await channel.SendAsync(
                message,
                new NotificationDestinationContext
                {
                    DestinationId = destination.Id,
                    Name = destination.Name,
                    Type = destination.Type,
                    ConfigurationJson = destination.ConfigurationJson
                },
                cancellationToken);

            return new TestNotificationDestinationResult
            {
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                Type = destination.Type,
                Success = true,
                Message = "Mensagem de teste enviada com sucesso."
            };
        }
        catch (Exception ex)
        {
            return new TestNotificationDestinationResult
            {
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                Type = destination.Type,
                Success = false,
                Message = "Falha ao enviar mensagem de teste.",
                Error = ex.Message
            };
        }
    }

    private static NotificationMessage BuildTestMessage()
    {
        return new NotificationMessage
        {
            AlertId = Guid.NewGuid(),
            Source = "central-alertas",
            Category = "teste",
            Type = "teste_destino",
            Severity = "info",
            Title = "Teste de destino de notificação",
            Message = "Esta é uma mensagem de teste enviada pela Central de Alertas.",
            MetricValue = null,
            MetricUnit = null,
            MetricThreshold = null,
            OccurrenceCount = 1,
            IsEscalating = false,
            LastSeenAt = DateTime.UtcNow
        };
    }
}