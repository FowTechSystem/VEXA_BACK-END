using MediatR;
using CRMVEXA.Application.Commands.Clientes;
using CRMVEXA.Infrastructure.Data;

namespace CRMVEXA.Application.Handlers.Clientes
{
    public class DeletarClienteCommandHandler : IRequestHandler<DeletarClienteCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeletarClienteCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletarClienteCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes.FindAsync(request.Id);

            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
