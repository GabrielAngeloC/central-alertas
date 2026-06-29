using CentralAlertas.Api.Contracts.Cors;
using CentralAlertas.Application.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/cors-origins")]
public class AllowedOriginsController : ControllerBase
{
    private readonly GetAllowedOriginsHandler _getHandler;
    private readonly GetAllowedOriginByIdHandler _getByIdHandler;
    private readonly CreateAllowedOriginHandler _createHandler;
    private readonly UpdateAllowedOriginHandler _updateHandler;
    private readonly DeleteAllowedOriginHandler _deleteHandler;
    private readonly ChangeAllowedOriginStatusHandler _changeStatusHandler;

    public AllowedOriginsController(
        GetAllowedOriginsHandler getHandler,
        GetAllowedOriginByIdHandler getByIdHandler,
        CreateAllowedOriginHandler createHandler,
        UpdateAllowedOriginHandler updateHandler,
        DeleteAllowedOriginHandler deleteHandler,
        ChangeAllowedOriginStatusHandler changeStatusHandler)
    {
        _getHandler = getHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _changeStatusHandler = changeStatusHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getHandler.HandleAsync(cancellationToken);
        return Ok(result.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(id, cancellationToken);

        if (result is null)
            return NotFound(new { message = "Origem não encontrada." });

        return Ok(MapToResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAllowedOriginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            request.Origin,
            request.Description,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Origin!.Id },
            MapToResponse(result.Origin));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAllowedOriginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            id,
            request.Origin,
            request.Description,
            request.IsActive,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.Origin!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.HandleAsync(id, cancellationToken);

        if (!deleted)
            return NotFound(new { message = "Origem não encontrada." });

        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.ActivateAsync(id, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.Origin!));
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.DeactivateAsync(id, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.Origin!));
    }

    private static AllowedOriginResponse MapToResponse(AllowedOriginResult origin)
    {
        return new AllowedOriginResponse
        {
            Id = origin.Id,
            Origin = origin.Origin,
            Description = origin.Description,
            IsActive = origin.IsActive,
            CreatedAt = origin.CreatedAt,
            UpdatedAt = origin.UpdatedAt
        };
    }
}
