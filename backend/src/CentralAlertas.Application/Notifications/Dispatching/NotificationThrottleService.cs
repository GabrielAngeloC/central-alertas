using CentralAlertas.Application.Notifications.Deliveries;

namespace CentralAlertas.Application.Notifications.Dispatching;

public class NotificationThrottleService
{
    private readonly IAlertDeliveryRepository _deliveryRepository;

    public NotificationThrottleService(
        IAlertDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
    }

    public async Task<NotificationThrottleResult> CheckAsync(
        Guid alertId,
        Guid destinationId,
        int? throttleMinutes,
        CancellationToken cancellationToken)
    {
        if (throttleMinutes is null || throttleMinutes <= 0)
        {
            return NotificationThrottleResult.Allowed();
        }

        var lastSuccessfulDelivery =
            await _deliveryRepository.GetLastSuccessfulDeliveryAsync(
                alertId,
                destinationId,
                cancellationToken);

        if (lastSuccessfulDelivery?.SentAt is null)
        {
            return NotificationThrottleResult.Allowed();
        }

        var nextAllowedAt = lastSuccessfulDelivery.SentAt.Value
            .AddMinutes(throttleMinutes.Value);

        var now = DateTime.UtcNow;

        if (nextAllowedAt <= now)
        {
            return NotificationThrottleResult.Allowed();
        }

        return NotificationThrottleResult.Blocked(nextAllowedAt);
    }
}

public class NotificationThrottleResult
{
    public bool IsAllowed { get; private set; }

    public DateTime? NextAllowedAt { get; private set; }

    public static NotificationThrottleResult Allowed()
    {
        return new NotificationThrottleResult
        {
            IsAllowed = true
        };
    }

    public static NotificationThrottleResult Blocked(DateTime nextAllowedAt)
    {
        return new NotificationThrottleResult
        {
            IsAllowed = false,
            NextAllowedAt = nextAllowedAt
        };
    }
}