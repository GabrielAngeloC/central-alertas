using System.Text.Json;

namespace CentralAlertas.Application.Notifications.Destinations;

public static class NotificationDestinationValidator
{
    private static readonly string[] AllowedTypes =
    [
        "telegram",
        "email"
    ];

    public static List<string> Validate(
        string name,
        string type,
        string configurationJson)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("O nome do destino é obrigatório.");

        if (string.IsNullOrWhiteSpace(type))
        {
            errors.Add("O tipo do destino é obrigatório.");
        }
        else if (!AllowedTypes.Contains(type.Trim().ToLower()))
        {
            errors.Add("O tipo do destino deve ser: telegram ou email.");
        }

        if (string.IsNullOrWhiteSpace(configurationJson))
        {
            errors.Add("A configuração do destino é obrigatória.");
        }
        else
        {
            ValidateJson(configurationJson, errors);
        }

        return errors;
    }

    private static void ValidateJson(
        string configurationJson,
        List<string> errors)
    {
        try
        {
            using var document = JsonDocument.Parse(configurationJson);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                errors.Add("A configuração deve ser um objeto JSON.");
            }
        }
        catch
        {
            errors.Add("A configuração deve ser um JSON válido.");
        }
    }
}