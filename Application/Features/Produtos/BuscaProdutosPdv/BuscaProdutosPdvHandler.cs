using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.BuscaProdutosPdv;

public record BuscaProdutosPdvRequest(string? Valor, int Limite = 30) : IRequest<List<BuscaProdutoPdvItem>>;

public class BuscaProdutoPdvItem
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "normal";
    public string Nome { get; set; } = "";
    public decimal PrecoVenda { get; set; }
    public int? Estoque { get; set; }
    public string? ConsignanteNome { get; set; }
    public string? Tamanho { get; set; }
    public string? Cor { get; set; }
}

public class BuscaProdutosPdvHandler : IRequestHandler<BuscaProdutosPdvRequest, List<BuscaProdutoPdvItem>>
{
    private readonly AppDbContext _db;
    private readonly IEmpresaContext _empresa;

    public BuscaProdutosPdvHandler(AppDbContext db, IEmpresaContext empresa)
    {
        _db = db;
        _empresa = empresa;
    }

    public async Task<List<BuscaProdutoPdvItem>> Handle(BuscaProdutosPdvRequest request, CancellationToken ct)
    {
        var empresaId = _empresa.EmpresaId;
        var limite = Math.Clamp(request.Limite <= 0 ? 30 : request.Limite, 1, 100);
        var termo = request.Valor?.Trim().ToLower() ?? "";

        IQueryable<Domain.Entities.Produto> normais = _db.Produtos.AsNoTracking()
            .Where(p => p.EmpresaId == empresaId);

        if (!string.IsNullOrEmpty(termo))
            normais = normais.Where(p =>
                p.Nome.ToLower().Contains(termo) ||
                (p.SKU != null && p.SKU.ToLower().Contains(termo)) ||
                (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(termo)));

        var normaisList = await normais
            .Include(p => p.Estoque)
            .OrderBy(p => p.Nome)
            .Take(limite)
            .Select(p => new BuscaProdutoPdvItem
            {
                Id = p.Id,
                Tipo = "normal",
                Nome = p.Nome,
                PrecoVenda = p.PrecoVenda,
                Estoque = p.Estoque != null ? p.Estoque.Quantidade : 0
            })
            .ToListAsync(ct);

        var restante = limite - normaisList.Count;
        if (restante <= 0) return normaisList;

        var bazarQuery = from p in _db.BazarProdutos.AsNoTracking()
                         join c in _db.BazarConsignantes.AsNoTracking() on p.ConsignanteId equals c.Id
                         where p.EmpresaId == empresaId && p.DeletadoEm == null &&
                               p.Status == BazarProdutoStatus.Disponivel && c.DeletadoEm == null
                         select new { p, c };

        if (!string.IsNullOrEmpty(termo))
            bazarQuery = bazarQuery.Where(x =>
                x.p.Descricao.ToLower().Contains(termo) ||
                x.c.Nome.ToLower().Contains(termo) ||
                (x.p.CodigoEtiqueta != null && x.p.CodigoEtiqueta.ToLower().Contains(termo)));

        var bazarList = await bazarQuery
            .OrderBy(x => x.p.Descricao)
            .Take(restante)
            .Select(x => new BuscaProdutoPdvItem
            {
                Id = x.p.Id,
                Tipo = "bazar",
                Nome = x.p.Descricao,
                PrecoVenda = x.p.ValorVenda,
                ConsignanteNome = x.c.Nome,
                Tamanho = x.p.Tamanho,
                Cor = x.p.Cor
            })
            .ToListAsync(ct);

        normaisList.AddRange(bazarList);
        return normaisList;
    }
}
