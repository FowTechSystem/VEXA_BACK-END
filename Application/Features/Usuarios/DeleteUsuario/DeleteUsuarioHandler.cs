using CRMVEXA.Application.Features.Usuarios.DeleteUsuario;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioHandler : IRequestHandler<DeleteUsuarioRequest, bool>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public DeleteUsuarioHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<bool> Handle(DeleteUsuarioRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var usuario = await _context.Usuarios
                .Where(u => u.EmpresaId == empresaId)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
