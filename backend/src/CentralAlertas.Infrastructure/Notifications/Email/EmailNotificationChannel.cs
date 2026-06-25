using System.Net;
using System.Net.Mail;
using System.Text.Json;
using CentralAlertas.Application.Notifications.Dispatching;
using Microsoft.Extensions.Options;

namespace CentralAlertas.Infrastructure.Notifications.Email;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly EmailOptions _options;

    public EmailNotificationChannel(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public string ChannelType => "email";

    public async Task SendAsync(
        NotificationMessage message,
        NotificationDestinationContext destination,
        CancellationToken cancellationToken)
    {
        ValidateOptions();

        var config = ParseConfiguration(destination.ConfigurationJson);

        if (config.Recipients.Count == 0)
            throw new InvalidOperationException("Destino de e-mail sem destinatários configurados.");

        var subject = BuildSubject(message);
        var body = BuildBody(message);

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        foreach (var recipient in config.Recipients)
        {
            mailMessage.To.Add(recipient);
        }

        using var smtpClient = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            EnableSsl = _options.UseSsl,
            Credentials = new NetworkCredential(
                _options.Username,
                _options.Password)
        };

        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.SmtpHost))
            throw new InvalidOperationException("SMTP host não configurado.");

        if (_options.SmtpPort <= 0)
            throw new InvalidOperationException("SMTP port inválida.");

        if (string.IsNullOrWhiteSpace(_options.Username))
            throw new InvalidOperationException("SMTP username não configurado.");

        if (string.IsNullOrWhiteSpace(_options.Password))
            throw new InvalidOperationException("SMTP password não configurado.");

        if (string.IsNullOrWhiteSpace(_options.FromEmail))
            throw new InvalidOperationException("E-mail remetente não configurado.");
    }

    private static EmailDestinationConfiguration ParseConfiguration(
        string configurationJson)
    {
        try
        {
            var config = JsonSerializer.Deserialize<EmailDestinationConfiguration>(
                configurationJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return config ?? new EmailDestinationConfiguration();
        }
        catch
        {
            throw new InvalidOperationException("Configuração de e-mail inválida.");
        }
    }

    private static string BuildSubject(NotificationMessage message)
    {
        return $"[{message.Severity.ToUpperInvariant()}] {message.Title}";
    }

    private static string BuildBody(NotificationMessage message)
    {
        return NotificationMessageFormatter.FormatPlainText(message);
    }
}

public class EmailDestinationConfiguration
{
    public List<string> Recipients { get; set; } = [];
}