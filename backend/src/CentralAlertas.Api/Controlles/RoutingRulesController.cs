using CentralAlertas.Api.Contracts.Notifications.Routing;
using CentralAlertas.Application.Notifications.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CentralAlertas.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/routing-rules")]
public class RoutingRulesController : ControllerBase
{
    private readonly GetRoutingRulesHandler _getHandler;
    private readonly CreateRoutingRuleHandler _createHandler;
    private readonly UpdateRoutingRuleHandler _updateHandler;
    private readonly RoutingEngine _routingEngine;
    public RoutingRulesController(
        GetRoutingRulesHandler getHandler,
        CreateRoutingRuleHandler createHandler,
        UpdateRoutingRuleHandler updateHandler,
        RoutingEngine routingEngine)
    {
        _getHandler = getHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _routingEngine = routingEngine;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var rules = await _getHandler.HandleAsync(cancellationToken);

        var response = rules
            .Select(MapToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoutingRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRoutingRuleCommand
        {
            Name = request.Name ?? string.Empty,
            Order = request.Order,
            Severity = request.Severity,
            Category = request.Category,
            Type = request.Type,
            Source = request.Source,
            DeliveryMode = request.DeliveryMode ?? "immediate",
            ThrottleMinutes = request.ThrottleMinutes,
            DestinationIds = request.DestinationIds
        };

        var result = await _createHandler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = "Regra inválida.",
                errors = result.Errors
            });
        }

        return Created(
            $"/api/v1/routing-rules/{result.Rule!.Id}",
            MapToResponse(result.Rule));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateRoutingRuleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRoutingRuleCommand
        {
            Id = id,
            Name = request.Name ?? string.Empty,
            Order = request.Order,
            Severity = request.Severity,
            Category = request.Category,
            Type = request.Type,
            Source = request.Source,
            DeliveryMode = request.DeliveryMode ?? "immediate",
            ThrottleMinutes = request.ThrottleMinutes,
            IsActive = request.IsActive,
            DestinationIds = request.DestinationIds
        };

        var result = await _updateHandler.HandleAsync(
            command,
            cancellationToken);

        if (result.WasNotFound)
        {
            return NotFound(new
            {
                message = "Regra não encontrada."
            });
        }

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = "Regra inválida.",
                errors = result.Errors
            });
        }

        return Ok(MapToResponse(result.Rule!));
    }

    private static RoutingRuleResponse MapToResponse(
        RoutingRuleResult rule)
    {
        return new RoutingRuleResponse
        {
            Id = rule.Id,
            Name = rule.Name,
            Order = rule.Order,
            Severity = rule.Severity,
            Category = rule.Category,
            Type = rule.Type,
            Source = rule.Source,
            DeliveryMode = rule.DeliveryMode,
            ThrottleMinutes = rule.ThrottleMinutes,
            IsActive = rule.IsActive,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt,
            Destinations = rule.Destinations
                .Select(destination => new RoutingRuleDestinationResponse
                {
                    DestinationId = destination.DestinationId,
                    Name = destination.Name,
                    Type = destination.Type
                })
                .ToList()
        };
    }
    [HttpPost("test")]
    public async Task<IActionResult> TestRouting(
    [FromBody] TestRoutingRuleRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Source) ||
            string.IsNullOrWhiteSpace(request.Category) ||
            string.IsNullOrWhiteSpace(request.Type) ||
            string.IsNullOrWhiteSpace(request.Severity))
        {
            return BadRequest(new
            {
                message = "Campos obrigatórios: source, category, type e severity."
            });
        }

        var decision = await _routingEngine.RouteAsync(
            new RouteAlertCommand
            {
                Source = request.Source,
                Category = request.Category,
                Type = request.Type,
                Severity = request.Severity
            },
            cancellationToken);

        return Ok(new RoutingDecisionResponse
        {
            Matched = decision.Matched,
            RuleId = decision.RuleId,
            RuleName = decision.RuleName,
            DeliveryMode = decision.DeliveryMode,
            ThrottleMinutes = decision.ThrottleMinutes,
            Destinations = decision.Destinations
                .Select(destination => new RoutingDecisionDestinationResponse
                {
                    DestinationId = destination.DestinationId,
                    Name = destination.Name,
                    Type = destination.Type,
                    ConfigurationJson = destination.ConfigurationJson
                })
                .ToList()
        });
    }
}