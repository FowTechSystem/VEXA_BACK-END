using CRMVEXA.Application.Features.Vendas.CancelarVenda;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Vendas.CancelarVenda
{
    public class CancelarVendaHandler : IRequestHandler<CancelarVendaRequest, Unit>
    {
        private readonly AppDbContext _context;

        public CancelarVendaHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CancelarVendaRequest request, CancellationToken cancellationToken)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

            if (venda == null)
            {
                throw new Exception($"Venda com ID {request.Id} não encontrada");
            }

            if (venda.Status == "Cancelada")
            {
                throw new Exception("Venda já está cancelada");
            }

            // Restaurar estoque
            foreach (var item in venda.Itens)
            {
                var produto = await _context.Produtos.FindAsync(new object[] { item.ProdutoId }, cancellationToken);
                if (produto != null)
                {
                    produto.QuantidadeEstoque += item.Quantidade;

                    // Registrar movimentação de estoque
                    var movimentacao = new MovimentacaoEstoque(
                        item.ProdutoId,
                        "Entrada",
                        item.Quantidade,
                        $"Cancelamento da venda {venda.CodigoVenda}",
                        request.UsuarioId
                    );
                    _context.MovimentacoesEstoque.Add(movimentacao);
                }
            }

            venda.Cancelar();
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
