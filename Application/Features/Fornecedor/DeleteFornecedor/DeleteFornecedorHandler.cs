using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Fornecedor.DeleteFornecedor;

public class DeleteFornecedorHandler : IRequestHandler<DeleteFornecedorRequest, bool>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public DeleteFornecedorHandler(AppDbContext appDbContext, IEmpresaContext empresaContext)
    {
        _context = appDbContext;
        _empresaContext = empresaContext;
    }

    public async Task<bool> Handle(DeleteFornecedorRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var fornecedor = await _context.Fornecedores
             .Where(f => f.EmpresaId == empresaId)
             .FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);

        if (fornecedor == null)
            return false;

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
