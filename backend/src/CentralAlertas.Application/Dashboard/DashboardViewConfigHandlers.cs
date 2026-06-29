using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Dashboard;

public class GetDashboardViewConfigsHandler
{
    private readonly IDashboardViewRepository _repository;

    public GetDashboardViewConfigsHandler(IDashboardViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DashboardViewConfigResult>> HandleAsync(CancellationToken cancellationToken)
    {
        var views = await _repository.GetAllAsync(cancellationToken);
        return views.Select(DashboardViewConfigResult.FromEntity).ToList();
    }
}

public class CreateDashboardViewHandler
{
    private readonly IDashboardViewRepository _repository;

    public CreateDashboardViewHandler(IDashboardViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardViewConfigCommandResult> HandleAsync(
        string? category,
        string? title,
        int order,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(category))
            return DashboardViewConfigCommandResult.Invalid("A categoria é obrigatória.");

        if (string.IsNullOrWhiteSpace(title))
            return DashboardViewConfigCommandResult.Invalid("O título é obrigatório.");

        var normalizedCategory = category.Trim();

        var existing = await _repository.GetByCategoryAsync(normalizedCategory, cancellationToken);
        if (existing is not null)
            return DashboardViewConfigCommandResult.Invalid("Já existe uma visão para essa categoria.");

        var view = new DashboardViewConfig(normalizedCategory, title.Trim(), order);

        await _repository.AddAsync(view, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return DashboardViewConfigCommandResult.Success(DashboardViewConfigResult.FromEntity(view));
    }
}

public class UpdateDashboardViewHandler
{
    private readonly IDashboardViewRepository _repository;

    public UpdateDashboardViewHandler(IDashboardViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardViewConfigCommandResult> HandleAsync(
        Guid id,
        string? category,
        string? title,
        int order,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var view = await _repository.GetByIdAsync(id, cancellationToken);
        if (view is null)
            return DashboardViewConfigCommandResult.NotFound();

        if (string.IsNullOrWhiteSpace(category))
            return DashboardViewConfigCommandResult.Invalid("A categoria é obrigatória.");

        if (string.IsNullOrWhiteSpace(title))
            return DashboardViewConfigCommandResult.Invalid("O título é obrigatório.");

        var normalizedCategory = category.Trim();

        var duplicate = await _repository.GetByCategoryAsync(normalizedCategory, cancellationToken);
        if (duplicate is not null && duplicate.Id != id)
            return DashboardViewConfigCommandResult.Invalid("Já existe outra visão para essa categoria.");

        view.Update(normalizedCategory, title.Trim(), order, isActive);

        await _repository.SaveChangesAsync(cancellationToken);

        return DashboardViewConfigCommandResult.Success(DashboardViewConfigResult.FromEntity(view));
    }
}

public class ChangeDashboardViewStatusHandler
{
    private readonly IDashboardViewRepository _repository;

    public ChangeDashboardViewStatusHandler(IDashboardViewRepository repository)
    {
        _repository = repository;
    }

    public Task<DashboardViewConfigCommandResult> ActivateAsync(Guid id, CancellationToken ct) =>
        ChangeAsync(id, activate: true, ct);

    public Task<DashboardViewConfigCommandResult> DeactivateAsync(Guid id, CancellationToken ct) =>
        ChangeAsync(id, activate: false, ct);

    private async Task<DashboardViewConfigCommandResult> ChangeAsync(
        Guid id,
        bool activate,
        CancellationToken cancellationToken)
    {
        var view = await _repository.GetByIdAsync(id, cancellationToken);
        if (view is null)
            return DashboardViewConfigCommandResult.NotFound();

        if (activate) view.Activate();
        else view.Deactivate();

        await _repository.SaveChangesAsync(cancellationToken);

        return DashboardViewConfigCommandResult.Success(DashboardViewConfigResult.FromEntity(view));
    }
}

public class DeleteDashboardViewHandler
{
    private readonly IDashboardViewRepository _repository;

    public DeleteDashboardViewHandler(IDashboardViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var view = await _repository.GetByIdAsync(id, cancellationToken);
        if (view is null)
            return false;

        view.Delete();
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
