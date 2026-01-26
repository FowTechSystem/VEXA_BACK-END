using CRMVEXA.Application.Features.Compras.DeleteCompra;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.DeleteCompra
{
    public class DeleteCompraHandler : IRequestHandler<DeleteCompraRequest, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteCompraHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCompraRequest request, CancellationToken cancellationToken)
        {
            var compra = await _context.Compras
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
            {
                throw new Exception($"Compra com ID {request.Id} não encontrada");
            }

            // Reverter estoque se necessário
            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos.FindAsync(new object[] { item.ProdutoId }, cancellationToken);
                if (produto != null && compra.Status == "Concluída")
                {
                    produto.QuantidadeEstoque -= item.Quantidade;
                    if (produto.QuantidadeEstoque < 0)
                    {
                        produto.QuantidadeEstoque = 0;
                    }
                }
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
