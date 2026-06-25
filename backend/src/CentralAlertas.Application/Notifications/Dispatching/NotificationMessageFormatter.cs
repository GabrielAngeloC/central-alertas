using System.Globalization;

namespace CentralAlertas.Application.Notifications.Dispatching;

public static class NotificationMessageFormatter
{
    public static string FormatPlainText(NotificationMessage message)
    {
        var severityLabel = message.Severity.ToUpperInvariant();
        var severityEmoji = GetSeverityEmoji(message.Severity);

        var metricText = BuildMetricText(message);
        var escalationText = message.IsEscalating ? " · 📈 escalando" : string.Empty;

        return $"""
        {severityEmoji} [{severityLabel}] {message.Title}

        {metricText}
        {message.Message}

        {message.Source} · {message.Category}/{message.Type} · {message.OccurrenceCount}ª ocorrência{escalationText}
        """;
    }

    private static string BuildMetricText(NotificationMessage message)
    {
        if (message.MetricValue is null || string.IsNullOrWhiteSpace(message.MetricUnit))
            return "Sem métrica informada.";

        var value = message.MetricValue.Value.ToString("0.####", CultureInfo.InvariantCulture);

        if (message.MetricThreshold is null)
            return $"{value} {message.MetricUnit}";

        var threshold = message.MetricThreshold.Value.ToString("0.####", CultureInfo.InvariantCulture);

        return $"{value} {message.MetricUnit} (limite: {threshold})";
    }

    private static string GetSeverityEmoji(string severity)
    {
        return severity switch
        {
            "critical" => "🔴",
            "warning" => "🟡",
            "info" => "🔵",
            _ => "⚪"
        };
    }
}