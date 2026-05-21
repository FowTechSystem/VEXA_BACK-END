using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Services.Bazar;

public record BazarConsignanteResumoDto(decimal Pendente, int EmEstoque, int Vendidas, int Devolvidas);

public class BazarResumoService
{
    private readonly AppDbContext _db;

    public BazarResumoService(AppDbContext db) => _db = db;

    public async Task<BazarConsignanteResumoDto> ObterResumoConsignanteAsync(int empresaId, int consignanteId, CancellationToken ct = default)
    {
        var produtos = _db.BazarProdutos.AsNoTracking()
            .Where(p => p.EmpresaId == empresaId && p.ConsignanteId == consignanteId && p.DeletadoEm == null);

        var emEstoque = await produtos.CountAsync(p => p.Status == BazarProdutoStatus.Disponivel, ct);
        var vendidas = await produtos.CountAsync(p => p.Status == BazarProdutoStatus.Vendida, ct);
        var devolvidas = await produtos.CountAsync(p => p.Status == BazarProdutoStatus.Devolvida, ct);

        var pendente = await (
            from p in produtos
            where p.Status == BazarProdutoStatus.Vendida
            where !_db.BazarRepasseItens.Any(ri =>
                ri.ProdutoId == p.Id &&
                _db.BazarRepasses.Any(r => r.Id == ri.RepasseId && r.DeletadoEm == null))
            select p.ValorCompra
        ).SumAsync(ct);

        return new(pendente, emEstoque, vendidas, devolvidas);
    }

    public async Task<Dictionary<int, BazarConsignanteResumoDto>> ObterResumosConsignantesAsync(
        int empresaId, IEnumerable<int> consignanteIds, CancellationToken ct = default)
    {
        var ids = consignanteIds.Distinct().ToList();
        if (ids.Count == 0) return new();

        var produtos = await _db.BazarProdutos.AsNoTracking()
            .Where(p => p.EmpresaId == empresaId && ids.Contains(p.ConsignanteId) && p.DeletadoEm == null)
            .Select(p => new { p.ConsignanteId, p.Status, p.ValorCompra, p.Id })
            .ToListAsync(ct);

        var emRepasse = await _db.BazarRepasseItens.AsNoTracking()
            .Where(ri => _db.BazarRepasses.Any(r => r.Id == ri.RepasseId && r.DeletadoEm == null && r.EmpresaId == empresaId))
            .Select(ri => ri.ProdutoId)
            .ToListAsync(ct);
        var emRepasseSet = emRepasse.ToHashSet();

        return ids.ToDictionary(id => id, id =>
        {
            var doConsignante = produtos.Where(p => p.ConsignanteId == id).ToList();
            return new BazarConsignanteResumoDto(
                Pendente: doConsignante
                    .Where(p => p.Status == BazarProdutoStatus.Vendida && !emRepasseSet.Contains(p.Id))
                    .Sum(p => p.ValorCompra),
                EmEstoque: doConsignante.Count(p => p.Status == BazarProdutoStatus.Disponivel),
                Vendidas: doConsignante.Count(p => p.Status == BazarProdutoStatus.Vendida),
                Devolvidas: doConsignante.Count(p => p.Status == BazarProdutoStatus.Devolvida));
        });
    }

    public async Task<(decimal PendenteTotal, int ConsignantesPendentes, int EmEstoque, int VendidoMes)> ObterResumoGeralAsync(
        int empresaId, CancellationToken ct = default)
    {
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1);

        var produtos = _db.BazarProdutos.AsNoTracking()
            .Where(p => p.EmpresaId == empresaId && p.DeletadoEm == null);

        var emEstoque = await produtos.CountAsync(p => p.Status == BazarProdutoStatus.Disponivel, ct);
        var vendidoMes = await produtos.CountAsync(p =>
            p.Status == BazarProdutoStatus.Vendida &&
            p.DataVenda >= inicioMes && p.DataVenda < fimMes, ct);

        var pendentesPorConsignante = await (
            from p in produtos
            where p.Status == BazarProdutoStatus.Vendida
            where !_db.BazarRepasseItens.Any(ri =>
                ri.ProdutoId == p.Id &&
                _db.BazarRepasses.Any(r => r.Id == ri.RepasseId && r.DeletadoEm == null))
            group p by p.ConsignanteId into g
            select new { ConsignanteId = g.Key, Total = g.Sum(x => x.ValorCompra) }
        ).Where(x => x.Total > 0).ToListAsync(ct);

        return (
            pendentesPorConsignante.Sum(x => x.Total),
            pendentesPorConsignante.Count,
            emEstoque,
            vendidoMes);
    }
}
