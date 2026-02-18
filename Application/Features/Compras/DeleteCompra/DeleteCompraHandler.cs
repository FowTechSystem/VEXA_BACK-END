using System;
using CRMVEXA.Application.Features.Compras.DeleteCompra;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.DeleteCompra
{
    public class DeleteCompraHandler : IRequestHandler<DeleteCompraRequest, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public DeleteCompraHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<Unit> Handle(DeleteCompraRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var compra = await _context.Compras
                .Where(c => c.EmpresaId == empresaId)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
            {
                throw new Exception($"Compra com ID {request.Id} não encontrada");
            }

            // Reverter estoque se necessário
            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos
                    .Include(p => p.Estoque)
                    .FirstOrDefaultAsync(p => p.Id == item.ProdutoId, cancellationToken);
                if (produto?.Estoque != null && compra.Status == "Concluída")
                {
                    produto.Estoque.AtualizarQuantidade(-item.Quantidade);
                    if (produto.Estoque.Quantidade < 0)
                        produto.Estoque.Quantidade = 0;
                }
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
