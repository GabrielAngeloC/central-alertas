namespace CentralAlertas.Application.Authentication;

public class LoginResult
{
    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }

    public Guid? UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? AccessToken { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public static LoginResult Success(
        Guid userId,
        string name,
        string email,
        string accessToken,
        DateTime expiresAt)
    {
        return new LoginResult
        {
            IsSuccess = true,
            UserId = userId,
            Name = name,
            Email = email,
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };
    }

    public static LoginResult Failure(string errorMessage)
    {
        return new LoginResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}