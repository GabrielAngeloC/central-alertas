namespace CentralAlertas.Domain.Entities;

// Configuração de uma visão do painel: vincula uma category a um título e ordem.
// Administrável pela interface — categorias novas ganham visão sem mudança de código.
public class DashboardViewConfig
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Category { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public int Order { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    private DashboardViewConfig()
    {
    }

    public DashboardViewConfig(string category, string title, int order)
    {
        Category = (category ?? string.Empty).Trim();
        Title = (title ?? string.Empty).Trim();
        Order = order;
    }

    public void Update(string category, string title, int order, bool isActive)
    {
        Category = (category ?? string.Empty).Trim();
        Title = (title ?? string.Empty).Trim();
        Order = order;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
