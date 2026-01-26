using CRMVEXA.Application.Features.Produtos.DeleteProduto;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Produtos.DeleteProduto
{
    public class DeleteProdutoHandler : IRequestHandler<DeleteProdutoRequest, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteProdutoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProdutoRequest request, CancellationToken cancellationToken)
        {
            var produto = await _context.Produtos.FindAsync(new object[] { request.Id }, cancellationToken);

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
