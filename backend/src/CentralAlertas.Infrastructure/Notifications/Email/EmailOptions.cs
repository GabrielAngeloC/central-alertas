namespace CentralAlertas.Infrastructure.Notifications.Email;

public class EmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = "Central de Alertas";

    public bool UseSsl { get; set; } = true;
}