using System.Text.Json;
using CentralAlertas.Api.Contracts.Alerts;
using CentralAlertas.Api.Security;
using CentralAlertas.Application.Alerts.Ingestion;
using CentralAlertas.Application.Alerts.Queries;
using Microsoft.AspNetCore.Mvc;
using CentralAlertas.Application.Alerts.Commands;
using Microsoft.AspNetCore.Authorization;

namespace CentralAlertas.Api.Controllers;
    
[Authorize]
[ApiController]
[Route("api/v1/alerts")]
public class AlertsController : ControllerBase
{
    private readonly CreateAlertHandler _createAlertHandler;
    private readonly GetActiveAlertsHandler _getActiveAlertsHandler;
    private readonly GetAlertByIdHandler _getAlertByIdHandler;
    private readonly GetAlertOccurrencesHandler _getAlertOccurrencesHandler;
    private readonly ResolveAlertHandler _resolveAlertHandler;
    private readonly GetResolvedAlertsHandler _getResolvedAlertsHandler;
    private readonly GetAlertsHandler _getAlertsHandler;

    public AlertsController(
        CreateAlertHandler createAlertHandler,
        GetAlertsHandler getAlertsHandler,
        GetActiveAlertsHandler getActiveAlertsHandler,
        GetResolvedAlertsHandler getResolvedAlertsHandler,
        GetAlertByIdHandler getAlertByIdHandler,
        GetAlertOccurrencesHandler getAlertOccurrencesHandler,
        ResolveAlertHandler resolveAlertHandler)
    {
        _createAlertHandler = createAlertHandler;
        _getAlertsHandler = getAlertsHandler;
        _getActiveAlertsHandler = getActiveAlertsHandler;
        _getResolvedAlertsHandler = getResolvedAlertsHandler;
        _getAlertByIdHandler = getAlertByIdHandler;
        _getAlertOccurrencesHandler = getAlertOccurrencesHandler;
        _resolveAlertHandler = resolveAlertHandler;
    }
    private static AlertSummaryResponse MapToSummaryResponse(
    Domain.Entities.Alert alert)
    {
        return new AlertSummaryResponse
        {
            Id = alert.Id,
            Source = alert.Source,
            Category = alert.Category,
            Type = alert.Type,
            Severity = alert.Severity,
            Title = alert.Title,
            Message = alert.Message,
            DedupKey = alert.DedupKey,
            MetricValue = alert.MetricValue,
            MetricUnit = alert.MetricUnit,
            MetricThreshold = alert.MetricThreshold,
            OccurrenceCount = alert.OccurrenceCount,
            FirstSeenAt = alert.FirstSeenAt,
            LastSeenAt = alert.LastSeenAt,
            ResolvedAt = alert.ResolvedAt,
            ResolutionReason = alert.ResolutionReason,
            IsActive = alert.IsActive,
            IsEscalating = alert.IsEscalating
        };
    }

    [Authorize]
    [HttpGet("resolved")]
    public async Task<IActionResult> GetResolved(
    [FromQuery] string? severity,
    [FromQuery] string? category,
    [FromQuery] string? source,
    CancellationToken cancellationToken)
    {
        var alerts = await _getResolvedAlertsHandler.HandleAsync(
            new GetResolvedAlertsQuery
            {
                Severity = severity,
                Category = category,
                Source = source
            },
            cancellationToken);

        var response = alerts
            .Select(MapToSummaryResponse)
            .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveAlerts(
    [FromQuery] string? severity,
    [FromQuery] string? category,
    [FromQuery] string? source,
    CancellationToken cancellationToken)
    {
        var query = new GetActiveAlertsQuery
        {
            Severity = severity,
            Category = category,
            Source = source
        };

        var alerts = await _getActiveAlertsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = alerts
            .Select(MapToSummaryResponse)
            .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] string status = "active",
    [FromQuery] string? severity = null,
    [FromQuery] string? category = null,
    [FromQuery] string? source = null,
    [FromQuery] DateTime? from = null,
    [FromQuery] DateTime? to = null,
    CancellationToken cancellationToken = default)
    {
        var alerts = await _getAlertsHandler.HandleAsync(
            new GetAlertsQuery
            {
                Status = status,
                Severity = severity,
                Category = category,
                Source = source,
                From = from,
                To = to
            },
            cancellationToken);

        var response = alerts
            .Select(MapToSummaryResponse)
            .ToList();

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAlertById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var alert = await _getAlertByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (alert is null)
            return NotFound(new { message = "Alerta não encontrado." });

        var response = new AlertDetailResponse
        {
            Id = alert.Id,
            Source = alert.Source,
            Category = alert.Category,
            Type = alert.Type,
            Severity = alert.Severity,
            Title = alert.Title,
            Message = alert.Message,
            DedupKey = alert.DedupKey,
            MetricValue = alert.MetricValue,
            MetricUnit = alert.MetricUnit,
            MetricThreshold = alert.MetricThreshold,
            Items = DeserializeJsonElement(alert.ItemsJson),
            Payload = DeserializeJsonElement(alert.PayloadJson),
            OccurrenceCount = alert.OccurrenceCount,
            FirstSeenAt = alert.FirstSeenAt,
            LastSeenAt = alert.LastSeenAt,
            LastNotifiedAt = alert.LastNotifiedAt,
            ResolvedAt = alert.ResolvedAt,
            ResolutionReason = alert.ResolutionReason,
            IsActive = alert.IsActive,
            IsEscalating = alert.IsEscalating
        };

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:guid}/occurrences")]
    public async Task<IActionResult> GetAlertOccurrences(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var alert = await _getAlertByIdHandler.HandleAsync(
            id,
            cancellationToken);

        if (alert is null)
            return NotFound(new { message = "Alerta não encontrado." });

        var occurrences = await _getAlertOccurrencesHandler.HandleAsync(
            id,
            cancellationToken);

        var response = occurrences.Select(occurrence => new AlertOccurrenceResponse
        {
            Id = occurrence.Id,
            AlertId = occurrence.AlertId,
            MetricValue = occurrence.MetricValue,
            MetricUnit = occurrence.MetricUnit,
            MetricThreshold = occurrence.MetricThreshold,
            Items = DeserializeJsonElement(occurrence.ItemsJson),
            Payload = DeserializeJsonElement(occurrence.PayloadJson),
            ReceivedAt = occurrence.ReceivedAt
        });

        return Ok(response);
    }

    [Authorize]
    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        [FromRoute] Guid id,
        [FromBody] ResolveAlertRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _resolveAlertHandler.HandleAsync(
            id,
            request.Reason,
            cancellationToken);

        if (result.WasNotFound)
        {
            return NotFound(new
            {
                message = "Alerta não encontrado."
            });
        }

        if (result.WasAlreadyResolved)
        {
            return BadRequest(new
            {
                message = "Alerta já está resolvido."
            });
        }

        return Ok(new
        {
            message = "Alerta resolvido com sucesso."
        });
    }


    [HttpPost]
    [ApiKey]
    public async Task<IActionResult> CreateAlert(
        [FromBody] CreateAlertRequest request,
        CancellationToken cancellationToken)
    {
        var errors = CreateAlertRequestValidator.Validate(request);

        if (errors.Count > 0)
        {
            return BadRequest(new
            {
                message = "Requisição inválida.",
                errors
            });
        }

        var command = new CreateAlertCommand
        {
            Source = request.Source!.Trim(),
            Category = request.Category!.Trim(),
            Type = request.Type!.Trim(),
            Severity = request.Severity!.Trim().ToLower(),
            Title = request.Title!.Trim(),
            Message = request.Message?.Trim(),
            DedupKey = request.DedupKey!.Trim(),

            MetricValue = request.Metric?.Value,
            MetricUnit = request.Metric?.Unit,
            MetricThreshold = request.Metric?.Threshold,

            ItemsJson = SerializeJsonElement(request.Items),
            PayloadJson = SerializeJsonElement(request.Payload)
        };

        var result = await _createAlertHandler.HandleAsync(
            command,
            cancellationToken);

        return Ok(new CreateAlertResponse
        {
            AlertId = result.AlertId,
            Status = result.Status,
            OccurrenceCount = result.OccurrenceCount,
            IsNewAlert = result.IsNewAlert,
            IsEscalating = result.IsEscalating
        });
    }

    private static string? SerializeJsonElement(JsonElement? element)
    {
        if (element is null)
            return null;

        if (element.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return null;

        return element.Value.GetRawText();
    }

    private static JsonElement? DeserializeJsonElement(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        using var document = JsonDocument.Parse(json);

        return document.RootElement.Clone();
    }
}