using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Alerts.Queries;

public class SearchAlertsHandler
{
    private readonly IAlertRepository _repository;

    public SearchAlertsHandler(IAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Alert>> HandleAsync(
        string term,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
            return [];

        return await _repository.SearchByItemAsync(term.Trim(), cancellationToken);
    }
}
