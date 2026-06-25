using CentralAlertas.Api.Contracts.Sources;
using CentralAlertas.Application.Sources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/sources")]
public class SourcesController : ControllerBase
{
    private readonly GetSourcesHandler _getSourcesHandler;
    private readonly GetSourceByIdHandler _getSourceByIdHandler;
    private readonly CreateSourceHandler _createSourceHandler;
    private readonly UpdateSourceHandler _updateSourceHandler;
    private readonly ChangeSourceStatusHandler _changeSourceStatusHandler;
    private readonly GetSourcesHealthHandler _getSourcesHealthHandler;
    private readonly CheckSilentSourcesHandler _checkSilentSourcesHandler;

    public SourcesController(
        GetSourcesHandler getSourcesHandler,
        GetSourceByIdHandler getSourceByIdHandler,
        CreateSourceHandler createSourceHandler,
        UpdateSourceHandler updateSourceHandler,
        ChangeSourceStatusHandler changeSourceStatusHandler,
        GetSourcesHealthHandler getSourcesHealthHandler,
        CheckSilentSourcesHandler checkSilentSourcesHandler)
    {
        _getSourcesHandler = getSourcesHandler;
        _getSourceByIdHandler = getSourceByIdHandler;
        _createSourceHandler = createSourceHandler;
        _updateSourceHandler = updateSourceHandler;
        _changeSourceStatusHandler = changeSourceStatusHandler;
        _getSourcesHealthHandler = getSourcesHealthHandler;
        _checkSilentSourcesHandler = checkSilentSourcesHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sources = await _getSourcesHandler.HandleAsync(cancellationToken);

        return Ok(sources);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var source = await _getSourceByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (source is null)
            return NotFound(new { message = "Source não encontrada." });

        return Ok(source);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSourceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createSourceHandler.HandleAsync(
            request.Name ?? string.Empty,
            request.ExpectedIntervalMinutes,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Source!.Id },
            result.Source);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSourceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateSourceHandler.HandleAsync(
            id,
            request.Name ?? string.Empty,
            request.ExpectedIntervalMinutes,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Source);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _changeSourceStatusHandler.ActivateAsync(
            id,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Source);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _changeSourceStatusHandler.DeactivateAsync(
            id,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Source);
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealth(CancellationToken cancellationToken)
    {
        var result = await _getSourcesHealthHandler.HandleAsync(cancellationToken);

        return Ok(result);
    }

    [HttpPost("check-silent")]
    public async Task<IActionResult> CheckSilent(CancellationToken cancellationToken)
    {
        var result = await _checkSilentSourcesHandler.HandleAsync(cancellationToken);

        return Ok(result);
    }
}