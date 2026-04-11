namespace CRMVEXA.Application.Features.Clientes;

/// <summary>Decodifica base64 puro ou data URL para bytes (persistência no banco).</summary>
public static class ClienteFotoPayloadHelper
{
    /// <summary>
    /// null = não decodificou (string vazia, path legado /uploads/, base64 inválido).
    /// </summary>
    public static byte[]? TryDecodeToBytes(string? fotoBase64OrDataUrlOrPath)
    {
        if (string.IsNullOrWhiteSpace(fotoBase64OrDataUrlOrPath))
            return null;

        var s = fotoBase64OrDataUrlOrPath.Trim();
        if (s.StartsWith("/uploads/", StringComparison.Ordinal))
            return null;

        if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var comma = s.IndexOf(',');
            if (comma < 0)
                return null;
            s = s[(comma + 1)..].Trim();
        }

        try
        {
            return Convert.FromBase64String(s);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
