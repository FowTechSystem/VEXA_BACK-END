using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace CRMVEXA.Application.Services.Bazar;

public class BazarFotoStorageService
{
    private readonly IWebHostEnvironment _env;
    private const int MaxEntradaBytes = 10 * 1024 * 1024;
    private const int MaxLadoPx = 800;

    public BazarFotoStorageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SalvarFotoAsync(int empresaId, int produtoId, int ordem, IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Arquivo vazio.");
        if (file.Length > MaxEntradaBytes)
            throw new InvalidOperationException("Arquivo muito grande. Tamanho máximo antes do processamento: 10MB.");
        if (file.ContentType == null || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Apenas imagens são permitidas.");

        await using var input = file.OpenReadStream();
        using var image = await Image.LoadAsync(input, ct);

        var ratio = (double)MaxLadoPx / Math.Max(image.Width, image.Height);
        if (ratio < 1)
        {
            var w = (int)Math.Round(image.Width * ratio);
            var h = (int)Math.Round(image.Height * ratio);
            image.Mutate(x => x.Resize(w, h));
        }

        var dir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "bazar", empresaId.ToString());
        Directory.CreateDirectory(dir);

        var fileName = $"produto-{produtoId}-{ordem}.jpg";
        var fullPath = Path.Combine(dir, fileName);

        await using var output = File.Create(fullPath);
        await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = 82 }, ct);

        return $"/bazar/{empresaId}/{fileName}";
    }
}
