namespace CentralAlertas.Application.Notifications.Dispatching;

public interface INotificationChannel
{
    string ChannelType { get; }

    Task SendAsync(
        NotificationMessage message,
        NotificationDestinationContext destination,
        CancellationToken cancellationToken);
}