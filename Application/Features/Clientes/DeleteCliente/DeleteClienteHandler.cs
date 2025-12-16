using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;

namespace CRMVEXA.Application.Features.Clientes.DeleteCliente;

public class DeleteClienteHandler
{
    private readonly AppDbContext _context;

    public DeleteClienteHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteClienteRequest request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes.FindAsync(new object[] { request.id }, cancellationToken);

        if (cliente == null)
            return false;

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

}
