namespace CentralAlertas.Application.Authentication;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;

    public string Issuer { get; set; } = "CentralAlertas";

    public string Audience { get; set; } = "CentralAlertasAdmin";

    public int ExpirationMinutes { get; set; } = 480;
}