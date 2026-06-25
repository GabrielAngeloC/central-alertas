namespace CentralAlertas.Application.Authentication;

public class LoginHandler
{
    private readonly IAppUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public LoginHandler(
        IAppUserRepository userRepository,
        PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> HandleAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return LoginResult.Failure("E-mail e senha são obrigatórios.");
        }

        var normalizedEmail = email.Trim().ToLower();

        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null)
            return LoginResult.Failure("E-mail ou senha inválidos.");

        if (!user.IsActive)
            return LoginResult.Failure("Usuário inativo.");

        var passwordIsValid = _passwordHasher.Verify(
            password,
            user.PasswordHash);

        if (!passwordIsValid)
            return LoginResult.Failure("E-mail ou senha inválidos.");

        var token = _jwtTokenGenerator.Generate(
            user.Id,
            user.Name,
            user.Email);

        return LoginResult.Success(
            user.Id,
            user.Name,
            user.Email,
            token.AccessToken,
            token.ExpiresAt);
    }
}