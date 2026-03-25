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
}
