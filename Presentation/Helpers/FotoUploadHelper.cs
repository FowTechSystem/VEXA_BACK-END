using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMVEXA.Presentation.Helpers;

/// <summary>Upload multipart de foto (campo <c>file</c>) → data URL para o front persistir no PUT/POST.</summary>
public static class FotoUploadHelper
{
    private static readonly string[] ExtensoesPermitidas = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif", ".avif"];
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024;

    public static async Task<ActionResult> ProcessarUploadAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return Erro("Nenhum arquivo enviado.");

        var extensao = ResolverExtensao(file);
        if (string.IsNullOrEmpty(extensao) || !ExtensoesPermitidas.Contains(extensao))
            return Erro("Formato de arquivo não permitido. Use JPG, PNG, GIF, WEBP ou AVIF.");

        if (file.Length > TamanhoMaximoBytes)
            return Erro("Arquivo muito grande. Tamanho máximo: 5MB.");

        try
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();
            var b64 = Convert.ToBase64String(bytes);
            var mime = ResolverMime(file, extensao);
            var dataUrl = $"data:{mime};base64,{b64}";

            return new OkObjectResult(new
            {
                sucesso = true,
                url = dataUrl,
                fotoUrl = dataUrl,
                fotoBase64 = b64
            });
        }
        catch (Exception ex)
        {
            return Erro(ex.Message);
        }
    }

    /// <summary>Extensão a partir do nome do arquivo; se vazio, infere do Content-Type (ex.: blob do navegador).</summary>
    private static string ResolverExtensao(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!string.IsNullOrEmpty(ext))
            return ext == ".jfif" ? ".jpeg" : ext;

        var contentType = file.ContentType?.Split(';')[0].Trim().ToLowerInvariant();
        return contentType switch
        {
            "image/jpeg" or "image/jpg" or "image/pjpeg" => ".jpeg",
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            "image/avif" => ".avif",
            _ => string.Empty
        };
    }

    private static string ResolverMime(IFormFile file, string extensao)
    {
        if (!string.IsNullOrWhiteSpace(file.ContentType) && file.ContentType != "application/octet-stream")
            return file.ContentType.Split(';')[0].Trim();

        return extensao switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".avif" => "image/avif",
            _ => "image/jpeg"
        };
    }

    private static BadRequestObjectResult Erro(string mensagem) =>
        new(new { sucesso = false, mensagem, message = mensagem });
}
