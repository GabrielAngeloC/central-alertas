using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Dashboard;

public class DashboardViewConfigResult
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static DashboardViewConfigResult FromEntity(DashboardViewConfig view)
    {
        return new DashboardViewConfigResult
        {
            Id = view.Id,
            Category = view.Category,
            Title = view.Title,
            Order = view.Order,
            IsActive = view.IsActive,
            CreatedAt = view.CreatedAt,
            UpdatedAt = view.UpdatedAt
        };
    }
}

public class DashboardViewConfigCommandResult
{
    public bool IsSuccess { get; set; }
    public bool WasNotFound { get; set; }
    public string? ErrorMessage { get; set; }
    public DashboardViewConfigResult? View { get; set; }

    public static DashboardViewConfigCommandResult Success(DashboardViewConfigResult view) =>
        new() { IsSuccess = true, View = view };

    public static DashboardViewConfigCommandResult Invalid(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };

    public static DashboardViewConfigCommandResult NotFound() =>
        new() { IsSuccess = false, WasNotFound = true, ErrorMessage = "Visão não encontrada." };
}
