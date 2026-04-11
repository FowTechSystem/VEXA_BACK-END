namespace CRMVEXA.Application.Features.Clientes;

/// <summary>
/// Bytes da coluna <see cref="Domain.Entities.Cliente.Foto"/> → data URL para o front (img src).
/// </summary>
public static class ClienteFotoUrlHelper
{
    public static string? ToApiFotoUrl(byte[]? foto)
    {
        if (foto == null || foto.Length == 0)
            return null;

        var mime = SniffImageMime(foto);
        return $"data:{mime};base64,{Convert.ToBase64String(foto)}";
    }

    private static string SniffImageMime(ReadOnlySpan<byte> data)
    {
        if (data.Length >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
            return "image/jpeg";
        if (data.Length >= 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
            return "image/png";
        if (data.Length >= 6 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46)
            return "image/gif";
        if (data.Length >= 12 && data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[8] == 0x57 && data[9] == 0x45 && data[10] == 0x42 && data[11] == 0x50)
            return "image/webp";
        return "image/jpeg";
    }
}
