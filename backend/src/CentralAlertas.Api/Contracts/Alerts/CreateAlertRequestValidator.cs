namespace CentralAlertas.Api.Contracts.Alerts;

public static class CreateAlertRequestValidator
{
    private static readonly string[] AllowedSeverities =
    [
        "critical",
        "warning",
        "info"
    ];

    public static List<string> Validate(CreateAlertRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Source))
            errors.Add("O campo 'source' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Category))
            errors.Add("O campo 'category' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Type))
            errors.Add("O campo 'type' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Severity))
        {
            errors.Add("O campo 'severity' é obrigatório.");
        }
        else if (!AllowedSeverities.Contains(request.Severity.Trim().ToLower()))
        {
            errors.Add("O campo 'severity' deve ser: critical, warning ou info.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add("O campo 'title' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.DedupKey))
            errors.Add("O campo 'dedup_key' é obrigatório.");

        if (request.Metric is not null && string.IsNullOrWhiteSpace(request.Metric.Unit))
            errors.Add("Quando 'metric' for informado, o campo 'metric.unit' deve ser informado.");

        return errors;
    }
}