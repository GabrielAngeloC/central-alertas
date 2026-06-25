using System.Net.Http.Json;
using System.Text.Json;
using CentralAlertas.Application.Notifications.Dispatching;
using Microsoft.Extensions.Options;

namespace CentralAlertas.Infrastructure.Notifications.Telegram;

public class TelegramNotificationChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;
    private readonly TelegramOptions _options;

    public TelegramNotificationChannel(
        HttpClient httpClient,
        IOptions<TelegramOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public string ChannelType => "telegram";

    public async Task SendAsync(
        NotificationMessage message,
        NotificationDestinationContext destination,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BotToken))
            throw new InvalidOperationException("Telegram BotToken não configurado.");

        var config = ParseConfiguration(destination.ConfigurationJson);

        if (string.IsNullOrWhiteSpace(config.ChatId))
            throw new InvalidOperationException("Destino Telegram sem chatId configurado.");

        var text = NotificationMessageFormatter.FormatPlainText(message);

        var url = $"/bot{_options.BotToken}/sendMessage";

        var payload = new
        {
            chat_id = config.ChatId,
            text,
            disable_web_page_preview = true
        };

        var response = await _httpClient.PostAsJsonAsync(
            url,
            payload,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"BotToken = '{_options.BotToken}'");
            throw new InvalidOperationException(
                $"Falha ao enviar Telegram. Status: {(int)response.StatusCode}. Body: {body}");
        }
    }

    private static TelegramDestinationConfiguration ParseConfiguration(
        string configurationJson)
    {
        try
        {
            var config = JsonSerializer.Deserialize<TelegramDestinationConfiguration>(
                configurationJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return config ?? new TelegramDestinationConfiguration();
        }
        catch
        {
            throw new InvalidOperationException("Configuração Telegram inválida.");
        }
    }
}

public class TelegramDestinationConfiguration
{
    public string ChatId { get; set; } = string.Empty;
}