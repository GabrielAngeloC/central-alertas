using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Cors;

public static class OriginValidator
{
    // Uma origem válida é scheme://host[:porta], sem caminho/query/fragmento.
    public static bool IsValid(string? origin, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(origin))
        {
            error = "A origem é obrigatória.";
            return false;
        }

        var normalized = AllowedOrigin.Normalize(origin);

        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            error = "Origem inválida. Use o formato http(s)://host[:porta].";
            return false;
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            error = "A origem deve usar http ou https.";
            return false;
        }

        // não pode ter caminho, query ou fragmento (apenas "/" implícito)
        if ((uri.AbsolutePath is not ("/" or "")) ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            error = "A origem não deve conter caminho, query ou fragmento.";
            return false;
        }

        return true;
    }
}
