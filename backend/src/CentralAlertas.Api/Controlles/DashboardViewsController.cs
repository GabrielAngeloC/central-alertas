using CentralAlertas.Api.Contracts.Dashboard;
using CentralAlertas.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/dashboard-views")]
public class DashboardViewsController : ControllerBase
{
    private readonly GetDashboardViewConfigsHandler _getHandler;
    private readonly CreateDashboardViewHandler _createHandler;
    private readonly UpdateDashboardViewHandler _updateHandler;
    private readonly ChangeDashboardViewStatusHandler _changeStatusHandler;
    private readonly DeleteDashboardViewHandler _deleteHandler;

    public DashboardViewsController(
        GetDashboardViewConfigsHandler getHandler,
        CreateDashboardViewHandler createHandler,
        UpdateDashboardViewHandler updateHandler,
        ChangeDashboardViewStatusHandler changeStatusHandler,
        DeleteDashboardViewHandler deleteHandler)
    {
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _changeStatusHandler = changeStatusHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getHandler.HandleAsync(cancellationToken);
        return Ok(result.Select(MapToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDashboardViewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createHandler.HandleAsync(
            request.Category,
            request.Title,
            request.Order,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Created(
            $"/api/v1/dashboard-views/{result.View!.Id}",
            MapToResponse(result.View));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDashboardViewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateHandler.HandleAsync(
            id,
            request.Category,
            request.Title,
            request.Order,
            request.IsActive,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.View!));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.HandleAsync(id, cancellationToken);

        if (!deleted)
            return NotFound(new { message = "Visão não encontrada." });

        return NoContent();
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.ActivateAsync(id, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.View!));
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.DeactivateAsync(id, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(MapToResponse(result.View!));
    }

    private static DashboardViewConfigResponse MapToResponse(DashboardViewConfigResult view)
    {
        return new DashboardViewConfigResponse
        {
            Id = view.Id,
            Category = view.Category,
            Title = view.Title,
            Order = view.Order,
            IsActive = view.IsActive,
            CreatedAt = view.CreatedAt,
            UpdatedAt = view.UpdatedAt
        };
    }
}
