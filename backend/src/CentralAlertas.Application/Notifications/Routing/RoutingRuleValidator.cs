namespace CentralAlertas.Application.Notifications.Routing;

public static class RoutingRuleValidator
{
    private static readonly string[] AllowedSeverities =
    [
        "critical",
        "warning",
        "info"
    ];

    private static readonly string[] AllowedDeliveryModes =
    [
        "immediate",
        "digest",
        "store_only"
    ];

    public static List<string> Validate(
        string name,
        int order,
        string? severity,
        string deliveryMode,
        int? throttleMinutes,
        List<Guid> destinationIds)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("O nome da regra é obrigatório.");

        if (order <= 0)
            errors.Add("A ordem da regra deve ser maior que zero.");

        if (!string.IsNullOrWhiteSpace(severity) &&
            !AllowedSeverities.Contains(severity.Trim().ToLower()))
        {
            errors.Add("A severidade deve ser: critical, warning ou info.");
        }

        if (string.IsNullOrWhiteSpace(deliveryMode))
        {
            errors.Add("O modo de entrega é obrigatório.");
        }
        else if (!AllowedDeliveryModes.Contains(deliveryMode.Trim().ToLower()))
        {
            errors.Add("O modo de entrega deve ser: immediate, digest ou store_only.");
        }

        if (throttleMinutes is not null && throttleMinutes < 0)
            errors.Add("O throttle não pode ser negativo.");

        if (deliveryMode != "store_only" && destinationIds.Count == 0)
            errors.Add("Regras que enviam notificação precisam ter ao menos um destino.");

        return errors;
    }
}