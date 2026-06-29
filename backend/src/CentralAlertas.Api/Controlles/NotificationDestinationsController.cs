using CentralAlertas.Api.Contracts.Notifications.Destinations;
using CentralAlertas.Application.Notifications.Destinations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/notification-destinations")]
public class NotificationDestinationsController : ControllerBase
{
    private readonly GetNotificationDestinationsHandler _getHandler;
    private readonly GetNotificationDestinationByIdHandler _getByIdHandler;
    private readonly CreateNotificationDestinationHandler _createHandler;
    private readonly UpdateNotificationDestinationHandler _updateHandler;
    private readonly TestNotificationDestinationHandler _testHandler;
    private readonly ChangeNotificationDestinationStatusHandler _changeStatusHandler;
    private readonly DeleteNotificationDestinationHandler _deleteHandler;

    public NotificationDestinationsController(
        GetNotificationDestinationsHandler getHandler,
        GetNotificationDestinationByIdHandler getByIdHandler,
        CreateNotificationDestinationHandler createHandler,
        UpdateNotificationDestinationHandler updateHandler,
        TestNotificationDestinationHandler testHandler,
        ChangeNotificationDestinationStatusHandler changeStatusHandler,
        DeleteNotificationDestinationHandler deleteHandler)
    {
        _getHandler = getHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _testHandler = testHandler;
        _changeStatusHandler = changeStatusHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _deleteHandler.HandleAsync(id, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (result.WasInUse)
            return Conflict(new { message = result.ErrorMessage });

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _getHandler.HandleAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (result is null)
            return NotFound(new { message = "Destino de notificação não encontrado." });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationDestinationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateNotificationDestinationCommand
        {
            Name = request.Name ?? string.Empty,
            Type = request.Type ?? string.Empty,
            ConfigurationJson = request.Configuration.HasValue
                ? request.Configuration.Value.GetRawText()
                : "{}"
        };

        var result = await _createHandler.HandleAsync(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Destination!.Id },
            result.Destination);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateNotificationDestinationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateNotificationDestinationCommand
        {
            Id = id,
            Name = request.Name ?? string.Empty,
            Type = request.Type ?? string.Empty,
            ConfigurationJson = request.Configuration.HasValue
                ? request.Configuration.Value.GetRawText()
                : "{}",
            IsActive = request.IsActive
        };

        var result = await _updateHandler.HandleAsync(command, cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(result.Destination);
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> Activate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.ActivateAsync(
            id,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Destination);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _changeStatusHandler.DeactivateAsync(
            id,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result.Destination);
    }

    [HttpPost("{id:guid}/test")]
    public async Task<IActionResult> Test(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _testHandler.HandleAsync(
            id,
            cancellationToken);

        if (result.WasNotFound)
            return NotFound(new { message = result.Error });

        if (!result.Success)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }
}