using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CentralAlertas.Api.Security;

public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string HeaderName = "X-API-KEY";

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>();

        var configuredApiKey = configuration["ApiKey:Value"];

        if (string.IsNullOrWhiteSpace(configuredApiKey))
        {
            context.Result = new ObjectResult(new
            {
                error = "API Key não configurada no servidor."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Header X-API-KEY não informado."
            });

            return;
        }

        if (!string.Equals(configuredApiKey, extractedApiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "API Key inválida."
            });

            return;
        }

        await next();
    }
}