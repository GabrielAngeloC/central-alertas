namespace CentralAlertas.Api.Contracts.Cors;

public class CreateAllowedOriginRequest
{
    public string? Origin { get; set; }

    public string? Description { get; set; }
}

public class UpdateAllowedOriginRequest
{
    public string? Origin { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

public class AllowedOriginResponse
{
    public Guid Id { get; set; }

    public string Origin { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
