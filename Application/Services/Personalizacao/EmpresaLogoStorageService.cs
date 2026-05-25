namespace CRMVEXA.Application.Services.Personalizacao;

public class EmpresaLogoStorageService
{
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/svg+xml", "image/jpeg", "image/webp"
    };

    private const int MaxBytes = 2 * 1024 * 1024;

    private readonly IWebHostEnvironment _env;

    public EmpresaLogoStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<(string Path, string Filename)> SalvarAsync(
        int empresaId,
        IFormFile file,
        CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Arquivo vazio.");
        if (file.Length > MaxBytes)
            throw new InvalidOperationException("Logo muito grande. Tamanho máximo: 2MB.");
        if (file.ContentType == null || !TiposPermitidos.Contains(file.ContentType))
            throw new InvalidOperationException("Formato não permitido. Use PNG, SVG, JPEG ou WebP.");

        var ext = ObterExtensao(file);
        var nomeArquivo = $"empresa-{empresaId}-{Guid.NewGuid():N}{ext}";
        var dir = ObterDiretorioLogos();
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, nomeArquivo);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, ct);

        return ($"/uploads/logos/{nomeArquivo}", file.FileName);
    }

    public void RemoverSeExistir(string? logoPath)
    {
        if (string.IsNullOrWhiteSpace(logoPath))
            return;

        var fileName = Path.GetFileName(logoPath.Trim().TrimStart('/'));
        if (string.IsNullOrEmpty(fileName))
            return;

        var fullPath = Path.Combine(ObterDiretorioLogos(), fileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string ObterDiretorioLogos() =>
        Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "logos");

    private static string ObterExtensao(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName);
        if (!string.IsNullOrWhiteSpace(ext))
            return ext.ToLowerInvariant();

        return file.ContentType switch
        {
            "image/png" => ".png",
            "image/svg+xml" => ".svg",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
    }
}
