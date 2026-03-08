using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Catalogo.DeleteCatalogo;

public class DeleteCatalogoHandler : IRequestHandler<DeleteCatalogoRequest, bool>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public DeleteCatalogoHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<bool> Handle(DeleteCatalogoRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível.");

        var empresaId = _empresaContext.EmpresaId;
        var catalogo = await _context.Catalogos
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.EmpresaId == empresaId, cancellationToken);

        if (catalogo == null)
            return false;

        catalogo.Desativar();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
