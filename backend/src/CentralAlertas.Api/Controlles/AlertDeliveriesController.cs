using CentralAlertas.Api.Contracts.Notifications.Deliveries;
using CentralAlertas.Application.Notifications.Deliveries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/alerts/{alertId:guid}/deliveries")]
public class AlertDeliveriesController : ControllerBase
{
    private readonly GetAlertDeliveriesHandler _getAlertDeliveriesHandler;

    public AlertDeliveriesController(
        GetAlertDeliveriesHandler getAlertDeliveriesHandler)
    {
        _getAlertDeliveriesHandler = getAlertDeliveriesHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAlertId(
        [FromRoute] Guid alertId,
        CancellationToken cancellationToken)
    {
        var deliveries = await _getAlertDeliveriesHandler.HandleAsync(
            alertId,
            cancellationToken);

        var response = deliveries
            .Select(delivery => new AlertDeliveryResponse
            {
                Id = delivery.Id,
                AlertId = delivery.AlertId,
                RoutingRuleId = delivery.RoutingRuleId,
                RoutingRuleName = delivery.RoutingRule?.Name,
                NotificationDestinationId = delivery.NotificationDestinationId,
                NotificationDestinationName = delivery.NotificationDestination?.Name,
                Channel = delivery.Channel,
                Status = delivery.Status,
                ErrorMessage = delivery.ErrorMessage,
                AttemptedAt = delivery.AttemptedAt,
                SentAt = delivery.SentAt
            })
            .ToList();

        return Ok(response);
    }
}