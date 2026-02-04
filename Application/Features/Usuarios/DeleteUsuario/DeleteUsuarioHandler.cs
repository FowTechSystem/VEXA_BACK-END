using CRMVEXA.Application.Features.Usuarios.DeleteUsuario;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioHandler : IRequestHandler<DeleteUsuarioRequest, bool>
    {
        private readonly AppDbContext _context;

        public DeleteUsuarioHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
