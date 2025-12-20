using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Fornecedor.DeleteFornecedor;

public class DeleteFornecedorHandler : IRequestHandler<DeleteFornecedorRequest, bool>
{
    private readonly AppDbContext _context;

    public DeleteFornecedorHandler(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<bool> Handle(DeleteFornecedorRequest request, CancellationToken cancellationToken)
    {
        var fornecedor = await _context.Fornecedores
             .FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);

        if (fornecedor == null)
            return false;

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
