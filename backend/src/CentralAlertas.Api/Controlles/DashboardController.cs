using CentralAlertas.Api.Contracts.Dashboard;
using CentralAlertas.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace CentralAlertas.Api.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly GetDashboardSummaryHandler _getDashboardSummaryHandler;
    private readonly GetDashboardViewsHandler _getDashboardViewsHandler;

    public DashboardController(
        GetDashboardSummaryHandler getDashboardSummaryHandler,
        GetDashboardViewsHandler getDashboardViewsHandler)
    {
        _getDashboardSummaryHandler = getDashboardSummaryHandler;
        _getDashboardViewsHandler = getDashboardViewsHandler;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var summary = await _getDashboardSummaryHandler.HandleAsync(
            cancellationToken);

        var response = new DashboardSummaryResponse
        {
            ActiveCriticalCount = summary.ActiveCriticalCount,
            ActiveWarningCount = summary.ActiveWarningCount,
            ActiveInfoCount = summary.ActiveInfoCount,
            ActiveAlertsCount = summary.ActiveAlertsCount,
            
            HealthySourcesCount = summary.HealthySourcesCount,
            SilentSourcesCount = summary.SilentSourcesCount,
            TotalSourcesCount = summary.TotalSourcesCount,

            AlertsBySeverity = summary.AlertsBySeverity
                .Select(x => new DashboardSeverityCountResponse
                {
                    Severity = x.Severity,
                    Count = x.Count
                })
                .ToList(),

            AlertsByCategory = summary.AlertsByCategory
                .Select(x => new DashboardCategoryCountResponse
                {
                    Category = x.Category,
                    Count = x.Count
                })
                .ToList(),

            LatestAlerts = summary.LatestAlerts
                .Select(alert => new DashboardLatestAlertResponse
                {
                    Id = alert.Id,
                    Source = alert.Source,
                    Category = alert.Category,
                    Type = alert.Type,
                    Severity = alert.Severity,
                    Title = alert.Title,
                    MetricValue = alert.MetricValue,
                    MetricUnit = alert.MetricUnit,
                    LastSeenAt = alert.LastSeenAt,
                    IsEscalating = alert.IsEscalating
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpGet("views")]
    public async Task<IActionResult> GetViews(CancellationToken cancellationToken)
    {
        var views = await _getDashboardViewsHandler.HandleAsync(
            cancellationToken);

        var response = views
            .Select(view => new DashboardViewResponse
            {
                Category = view.Category,
                Title = view.Title,
                Order = view.Order,
                Alerts = view.Alerts
                    .Select(alert => new DashboardViewAlertResponse
                    {
                        Id = alert.Id,
                        Source = alert.Source,
                        Type = alert.Type,
                        Severity = alert.Severity,
                        Title = alert.Title,
                        Message = alert.Message,
                        MetricValue = alert.MetricValue,
                        MetricUnit = alert.MetricUnit,
                        MetricThreshold = alert.MetricThreshold,
                        OccurrenceCount = alert.OccurrenceCount,
                        IsEscalating = alert.IsEscalating,
                        FirstSeenAt = alert.FirstSeenAt,
                        LastSeenAt = alert.LastSeenAt
                    })
                    .ToList()
            })
            .ToList();

        return Ok(response);
    }
}