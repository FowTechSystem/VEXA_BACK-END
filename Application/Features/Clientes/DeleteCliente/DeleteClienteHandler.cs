using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Clientes.DeleteCliente;

public class DeleteClienteHandler : IRequestHandler<DeleteClienteRequest, bool>  
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public DeleteClienteHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<bool> Handle(DeleteClienteRequest request, CancellationToken cancellationToken)
    {
        var empresaId = _empresaContext.EmpresaId;
        var cliente = await _context.Clientes
            .Where(c => c.EmpresaId == empresaId)
            .FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);

        if (cliente == null)
            return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
