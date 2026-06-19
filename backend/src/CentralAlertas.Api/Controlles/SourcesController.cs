using CentralAlertas.Api.Contracts.Sources;
using CentralAlertas.Application.Sources;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[ApiController]
[Route("api/v1/sources")]
public class SourcesController : ControllerBase
{
    private readonly GetSourcesHandler _getSourcesHandler;
    private readonly GetSourcesHealthHandler _getSourcesHealthHandler;

    public SourcesController(
        GetSourcesHandler getSourcesHandler,
        GetSourcesHealthHandler getSourcesHealthHandler)
    {
        _getSourcesHandler = getSourcesHandler;
        _getSourcesHealthHandler = getSourcesHealthHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetSources(CancellationToken cancellationToken)
    {
        var sources = await _getSourcesHandler.HandleAsync(cancellationToken);

        var response = sources
            .Select(source => new SourceResponse
            {
                Id = source.Id,
                Name = source.Name,
                ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
                LastReceivedAt = source.LastReceivedAt,
                IsActive = source.IsActive,
                CreatedAt = source.CreatedAt
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetSourcesHealth(
        CancellationToken cancellationToken)
    {
        var health = await _getSourcesHealthHandler.HandleAsync(
            cancellationToken);

        var response = health
            .Select(source => new SourceHealthResponse
            {
                Id = source.Id,
                Name = source.Name,
                ExpectedIntervalMinutes = source.ExpectedIntervalMinutes,
                LastReceivedAt = source.LastReceivedAt,
                NextExpectedAt = source.NextExpectedAt,
                Status = source.Status,
                MinutesLate = source.MinutesLate,
                IsSilent = source.IsSilent,
                IsActive = source.IsActive
            })
            .ToList();

        return Ok(response);
    }
}