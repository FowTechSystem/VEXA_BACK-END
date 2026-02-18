using System;
using CRMVEXA.Application.Features.Produtos.DeleteProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.DeleteProduto
{
    public class DeleteProdutoHandler : IRequestHandler<DeleteProdutoRequest, Unit>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public DeleteProdutoHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<Unit> Handle(DeleteProdutoRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
