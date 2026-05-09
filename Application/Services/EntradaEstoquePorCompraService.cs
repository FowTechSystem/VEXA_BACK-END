using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Services;

public class EntradaEstoquePorCompraService : IEntradaEstoquePorCompraService
{
    private readonly AppDbContext _context;

    public EntradaEstoquePorCompraService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarEntradaAsync(int empresaId, Compra compra, int usuarioId, string motivo, CancellationToken cancellationToken)
    {
        if (compra.Id <= 0)
            throw new InvalidOperationException("Compra deve estar persistida antes de registrar entrada no estoque.");

        var jaTemEntrada = await _context.MovimentacoesEstoque
            .AnyAsync(m => m.CompraId == compra.Id && m.TipoMovimentacao == "Entrada", cancellationToken);
        if (jaTemEntrada)
            return;

        foreach (var item in compra.Itens)
        {
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId && p.Id == item.ProdutoId)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(cancellationToken);

            if (produto == null)
                continue;

            if (produto.Estoque != null)
                produto.Estoque.AtualizarQuantidade(item.Quantidade);
            else
            {
                var estoque = new Estoque(empresaId, item.ProdutoId, item.Quantidade);
                _context.Estoques.Add(estoque);
            }

            var movimentacao = new MovimentacaoEstoque(
                empresaId,
                item.ProdutoId,
                "Entrada",
                item.Quantidade,
                motivo,
                usuarioId,
                compraId: compra.Id);

            _context.MovimentacoesEstoque.Add(movimentacao);
        }
    }

    public async Task EstornarEntradaPorCompraAsync(int empresaId, Compra compra, int usuarioId, CancellationToken cancellationToken)
    {
        var prefixoEstorno = $"Estorno (cancelamento) compra #{compra.Id}";
        var jaEstornado = await _context.MovimentacoesEstoque
            .AnyAsync(m => m.CompraId == compra.Id && m.Motivo.StartsWith(prefixoEstorno), cancellationToken);
        if (jaEstornado)
            return;

        var entradas = await _context.MovimentacoesEstoque
            .Where(m => m.EmpresaId == empresaId && m.CompraId == compra.Id && m.TipoMovimentacao == "Entrada")
            .ToListAsync(cancellationToken);

        if (entradas.Count == 0)
            return;

        foreach (var e in entradas)
        {
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId && p.Id == e.ProdutoId)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(cancellationToken);

            if (produto?.Estoque == null)
                throw new InvalidOperationException($"Não é possível estornar a compra: produto {e.ProdutoId} sem estoque cadastrado.");

            if (produto.Estoque.Quantidade < e.Quantidade)
                throw new InvalidOperationException(
                    $"Não é possível estornar a compra {compra.CodigoCompra}: saldo insuficiente para o produto «{produto.Nome}». Verifique vendas ou ajustes manuais.");

            produto.Estoque.AtualizarQuantidade(-e.Quantidade);

            var saida = new MovimentacaoEstoque(
                empresaId,
                e.ProdutoId,
                "Saída",
                e.Quantidade,
                $"{prefixoEstorno} ({compra.CodigoCompra})",
                usuarioId,
                compraId: compra.Id);

            _context.MovimentacoesEstoque.Add(saida);
        }
    }
}
