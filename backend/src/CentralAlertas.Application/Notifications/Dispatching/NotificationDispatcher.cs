using CentralAlertas.Application.Notifications.Deliveries;
using CentralAlertas.Application.Notifications.Routing;
using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Notifications.Dispatching;

public class NotificationDispatcher
{
    private readonly RoutingEngine _routingEngine;
    private readonly IAlertDeliveryRepository _deliveryRepository;
    private readonly IEnumerable<INotificationChannel> _channels;
    private readonly NotificationThrottleService _throttleService;

    public NotificationDispatcher(
        RoutingEngine routingEngine,
        IAlertDeliveryRepository deliveryRepository,
        IEnumerable<INotificationChannel> channels,
        NotificationThrottleService throttleService)
    {
        _routingEngine = routingEngine;
        _deliveryRepository = deliveryRepository;
        _channels = channels;
        _throttleService = throttleService;
    }

    public async Task<NotificationDispatchResult> DispatchAsync(
        Alert alert,
        CancellationToken cancellationToken)
    {
        var decision = await _routingEngine.RouteAsync(
            new RouteAlertCommand
            {
                Source = alert.Source,
                Category = alert.Category,
                Type = alert.Type,
                Severity = alert.Severity
            },
            cancellationToken);

        if (!decision.Matched)
        {
            var skipped = new AlertDelivery(
                alert.Id,
                routingRuleId: null,
                notificationDestinationId: null,
                channel: "none",
                status: "skipped",
                errorMessage: "Nenhuma regra de roteamento encontrada.");

            await _deliveryRepository.AddAsync(skipped, cancellationToken);
            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            return new NotificationDispatchResult
            {
                AlertId = alert.Id,
                Matched = false,
                DeliveryMode = "store_only",
                DeliveriesCreated = 1
            };
        }

        if (decision.DeliveryMode == "store_only")
        {
            var skipped = new AlertDelivery(
                alert.Id,
                decision.RuleId,
                notificationDestinationId: null,
                channel: "none",
                status: "skipped",
                errorMessage: "Regra configurada como store_only.");

            await _deliveryRepository.AddAsync(skipped, cancellationToken);
            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            return new NotificationDispatchResult
            {
                AlertId = alert.Id,
                Matched = true,
                RuleId = decision.RuleId,
                RuleName = decision.RuleName,
                DeliveryMode = decision.DeliveryMode,
                DeliveriesCreated = 1
            };
        }

        if (decision.DeliveryMode == "digest")
        {
            var skipped = new AlertDelivery(
                alert.Id,
                decision.RuleId,
                notificationDestinationId: null,
                channel: "digest",
                status: "skipped",
                errorMessage: "Entrega digest será processada por worker específico em aula futura.");

            await _deliveryRepository.AddAsync(skipped, cancellationToken);
            await _deliveryRepository.SaveChangesAsync(cancellationToken);

            return new NotificationDispatchResult
            {
                AlertId = alert.Id,
                Matched = true,
                RuleId = decision.RuleId,
                RuleName = decision.RuleName,
                DeliveryMode = decision.DeliveryMode,
                DeliveriesCreated = 1
            };
        }

        var message = BuildNotificationMessage(alert);
        var deliveriesCreated = 0;

        foreach (var destination in decision.Destinations)
        {
            var throttle = await _throttleService.CheckAsync(
                alert.Id,
                destination.DestinationId,
                decision.ThrottleMinutes,
                cancellationToken);

            if (!throttle.IsAllowed)
            {
                var skipped = new AlertDelivery(
                    alert.Id,
                    decision.RuleId,
                    destination.DestinationId,
                    destination.Type,
                    status: "skipped",
                    errorMessage: $"Throttle ativo. Próximo envio permitido em {throttle.NextAllowedAt:O}.");

                await _deliveryRepository.AddAsync(skipped, cancellationToken);
                deliveriesCreated++;
                continue;
            }

            var channel = _channels.FirstOrDefault(x => x.ChannelType == destination.Type);

            if (channel is null)
            {
                var failed = new AlertDelivery(
                    alert.Id,
                    decision.RuleId,
                    destination.DestinationId,
                    destination.Type,
                    status: "failed",
                    errorMessage: $"Nenhum canal registrado para o tipo '{destination.Type}'.");

                await _deliveryRepository.AddAsync(failed, cancellationToken);
                deliveriesCreated++;
                continue;
            }

            try
            {
                await channel.SendAsync(
                    message,
                    new NotificationDestinationContext
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name,
                        Type = destination.Type,
                        ConfigurationJson = destination.ConfigurationJson
                    },
                    cancellationToken);

                var success = new AlertDelivery(
                    alert.Id,
                    decision.RuleId,
                    destination.DestinationId,
                    destination.Type,
                    status: "success");

                await _deliveryRepository.AddAsync(success, cancellationToken);
            }
            catch (Exception ex)
            {
                var failed = new AlertDelivery(
                    alert.Id,
                    decision.RuleId,
                    destination.DestinationId,
                    destination.Type,
                    status: "failed",
                    errorMessage: ex.Message);

                await _deliveryRepository.AddAsync(failed, cancellationToken);
            }

            deliveriesCreated++;
        }

        await _deliveryRepository.SaveChangesAsync(cancellationToken);

        return new NotificationDispatchResult
        {
            AlertId = alert.Id,
            Matched = true,
            RuleId = decision.RuleId,
            RuleName = decision.RuleName,
            DeliveryMode = decision.DeliveryMode,
            DeliveriesCreated = deliveriesCreated
        };
    }

    private static NotificationMessage BuildNotificationMessage(Alert alert)
    {
        return new NotificationMessage
        {
            AlertId = alert.Id,
            Source = alert.Source,
            Category = alert.Category,
            Type = alert.Type,
            Severity = alert.Severity,
            Title = alert.Title,
            Message = alert.Message,
            MetricValue = alert.MetricValue,
            MetricUnit = alert.MetricUnit,
            MetricThreshold = alert.MetricThreshold,
            OccurrenceCount = alert.OccurrenceCount,
            IsEscalating = alert.IsEscalating,
            LastSeenAt = alert.LastSeenAt
        };
    }
}

public class NotificationDispatchResult
{
    public Guid AlertId { get; set; }

    public bool Matched { get; set; }

    public Guid? RuleId { get; set; }

    public string? RuleName { get; set; }

    public string DeliveryMode { get; set; } = string.Empty;

    public int DeliveriesCreated { get; set; }
}