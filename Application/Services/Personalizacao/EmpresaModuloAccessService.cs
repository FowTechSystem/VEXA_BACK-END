using CRMVEXA.Application.Constants;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Services.Personalizacao;

public class EmpresaModuloAccessService
{
    private readonly AppDbContext _db;
    private HashSet<string>? _cache;

    public EmpresaModuloAccessService(AppDbContext db) => _db = db;

    public async Task<bool> EstaAtivoAsync(string moduleKey, CancellationToken ct = default)
    {
        var ativos = await ObterModulosAtivosAsync(ct);
        return ativos.Contains(moduleKey, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<HashSet<string>> ObterModulosAtivosAsync(CancellationToken ct = default)
    {
        if (_cache != null)
            return _cache;

        var rows = await _db.EmpresaModulos.AsNoTracking()
            .Where(m => m.IsEnabled)
            .Select(m => m.ModuleKey)
            .ToListAsync(ct);

        var map = ModuloCatalogo.MesclarComPadrao(rows.Select(k => new EmpresaModuloEstado(k, true)));
        _cache = map.Where(kv => kv.Value).Select(kv => kv.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return _cache;
    }
}
