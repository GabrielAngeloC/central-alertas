using CentralAlertas.Api.Contracts.Authentication;
using CentralAlertas.Application.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginHandler _loginHandler;

    public AuthController(LoginHandler loginHandler)
    {
        _loginHandler = loginHandler;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _loginHandler.HandleAsync(
            request.Email ?? string.Empty,
            request.Password ?? string.Empty,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new
            {
                message = result.ErrorMessage
            });
        }

        return Ok(new LoginResponse
        {
            UserId = result.UserId!.Value,
            Name = result.Name!,
            Email = result.Email!,
            AccessToken = result.AccessToken!,
            ExpiresAt = result.ExpiresAt!.Value,
            TokenType = "Bearer"
        });
    }
}