using System;
using CRMVEXA.Application.Features.Compras.DeleteCompra;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Compras.DeleteCompra
{
    public class DeleteCompraHandler : IRequestHandler<DeleteCompraRequest, DeleteCompraResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public DeleteCompraHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<DeleteCompraResponse> Handle(DeleteCompraRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var compra = await _context.Compras
                .Where(c => c.EmpresaId == empresaId)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (compra == null)
                return new DeleteCompraResponse { Sucesso = false, Mensagem = "Compra não encontrada", Codigo = 404 };

            var statusNormalizado = compra.Status?.Trim().ToLowerInvariant();
            if (statusNormalizado == "recebida")
                return new DeleteCompraResponse { Sucesso = false, Mensagem = "Compra já recebida não pode ser excluída (possui movimentação de estoque)", Codigo = 409 };

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeleteCompraResponse { Sucesso = true, Codigo = 200 };
        }
    }
}
