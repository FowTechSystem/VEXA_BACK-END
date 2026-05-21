using CRMVEXA.Application.Features.Vendas.GetVenda;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas;

public static class VendaItemMapper
{
    public static async Task<List<ItemVendaResponse>> MapItensAsync(
        AppDbContext db,
        int empresaId,
        IEnumerable<ItemVenda> itens,
        CancellationToken ct = default)
    {
        var lista = itens.ToList();
        if (lista.Count == 0) return new();

        var produtoIds = lista.Where(i => i.ProdutoId.HasValue).Select(i => i.ProdutoId!.Value).Distinct().ToList();
        var bazarIds = lista.Where(i => i.BazarProdutoId.HasValue).Select(i => i.BazarProdutoId!.Value).Distinct().ToList();

        var produtos = produtoIds.Count > 0
            ? await db.Produtos.AsNoTracking()
                .Include(p => p.Categoria)
                .Where(p => p.EmpresaId == empresaId && produtoIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct)
            : new Dictionary<int, Produto>();

        var pecasBazar = bazarIds.Count > 0
            ? await db.BazarProdutos.AsNoTracking()
                .Include(p => p.Consignante)
                .Where(p => p.EmpresaId == empresaId && bazarIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct)
            : new Dictionary<int, BazarProduto>();

        return lista.Select(i => MapItem(i, produtos, pecasBazar)).ToList();
    }

    public static ItemVendaResponse MapItem(
        ItemVenda i,
        IReadOnlyDictionary<int, Produto> produtos,
        IReadOnlyDictionary<int, BazarProduto> pecasBazar)
    {
        var tipo = string.IsNullOrWhiteSpace(i.TipoItem) ? "normal" : i.TipoItem.Trim().ToLowerInvariant();
        if (i.BazarProdutoId.HasValue) tipo = "bazar";

        ProdutoRefResponse? produtoRef = null;
        string? consignanteNome = null;

        if (tipo == "bazar" && i.BazarProdutoId.HasValue && pecasBazar.TryGetValue(i.BazarProdutoId.Value, out var peca))
        {
            consignanteNome = peca.Consignante?.Nome;
            produtoRef = new ProdutoRefResponse
            {
                Id = peca.Id,
                Nome = peca.Descricao,
                Sku = "BAZAR",
                PrecoVenda = peca.ValorVenda,
                PrecoCusto = peca.ValorCompra,
                Categoria = null
            };
        }
        else if (i.ProdutoId.HasValue && produtos.TryGetValue(i.ProdutoId.Value, out var produto))
        {
            produtoRef = new ProdutoRefResponse
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Sku = produto.SKU ?? "",
                PrecoVenda = produto.PrecoVenda,
                PrecoCusto = produto.PrecoCusto,
                Categoria = produto.Categoria != null
                    ? new CategoriaRefResponse { Id = produto.Categoria.Id, Nome = produto.Categoria.Nome }
                    : null
            };
        }

        return new ItemVendaResponse
        {
            Id = i.Id,
            Tipo = tipo,
            ProdutoId = i.ProdutoId ?? i.BazarProdutoId ?? 0,
            BazarProdutoId = i.BazarProdutoId,
            ConsignanteNome = consignanteNome,
            Produto = produtoRef,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario,
            Subtotal = i.Subtotal
        };
    }
}
