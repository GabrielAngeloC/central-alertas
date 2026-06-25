using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class GetAlertsHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertsHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public Task<List<Alert>> HandleAsync(
        GetAlertsQuery query,
        CancellationToken cancellationToken)
    {
        var normalizedStatus = NormalizeStatus(query.Status);

        return _alertRepository.GetAsync(
            normalizedStatus,
            query.Severity,
            query.Category,
            query.Source,
            query.From,
            query.To,
            cancellationToken);
    }

    private static string NormalizeStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return "active";

        var normalized = status.Trim().ToLower();

        if (normalized is "active" or "resolved" or "all")
            return normalized;

        return "active";
    }
}