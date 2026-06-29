namespace CentralAlertas.Domain.Entities;

// Origem (CORS) permitida para o frontend. Gerenciada via CRUD e persistida no banco.
public class AllowedOrigin
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Origin { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    private AllowedOrigin()
    {
    }

    public AllowedOrigin(string origin, string? description)
    {
        Origin = Normalize(origin);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void Update(string origin, string? description, bool isActive)
    {
        Origin = Normalize(origin);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
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

    // Normaliza uma origem: minúscula no esquema/host, sem barra final.
    public static string Normalize(string origin)
    {
        var trimmed = (origin ?? string.Empty).Trim();
        return trimmed.TrimEnd('/');
    }
}
