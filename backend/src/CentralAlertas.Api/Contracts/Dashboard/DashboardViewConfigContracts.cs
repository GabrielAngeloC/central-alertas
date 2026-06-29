namespace CentralAlertas.Api.Contracts.Dashboard;

public class CreateDashboardViewRequest
{
    public string? Category { get; set; }
    public string? Title { get; set; }
    public int Order { get; set; }
}

public class UpdateDashboardViewRequest
{
    public string? Category { get; set; }
    public string? Title { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DashboardViewConfigResponse
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
