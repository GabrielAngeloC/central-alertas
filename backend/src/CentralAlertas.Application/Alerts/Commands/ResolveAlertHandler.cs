using CentralAlertas.Application.Alerts;

namespace CentralAlertas.Application.Alerts.Commands;

public class ResolveAlertHandler
{
    private readonly IAlertRepository _alertRepository;

    public ResolveAlertHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<ResolveAlertResult> HandleAsync(
        Guid alertId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var alert = await _alertRepository.GetByIdAsync(
            alertId,
            cancellationToken);

        if (alert is null)
        {
            return ResolveAlertResult.NotFound();
        }

        if (!alert.IsActive)
        {
            return ResolveAlertResult.AlreadyResolved();
        }

        alert.Resolve(reason);

        await _alertRepository.SaveChangesAsync(cancellationToken);

        return ResolveAlertResult.Success();
    }
}

public class ResolveAlertResult
{
    public bool IsSuccess { get; private set; }

    public bool WasNotFound { get; private set; }

    public bool WasAlreadyResolved { get; private set; }

    public static ResolveAlertResult Success()
    {
        return new ResolveAlertResult
        {
            IsSuccess = true
        };
    }

    public static ResolveAlertResult NotFound()
    {
        return new ResolveAlertResult
        {
            IsSuccess = false,
            WasNotFound = true
        };
    }

    public static ResolveAlertResult AlreadyResolved()
    {
        return new ResolveAlertResult
        {
            IsSuccess = false,
            WasAlreadyResolved = true
        };
    }
}