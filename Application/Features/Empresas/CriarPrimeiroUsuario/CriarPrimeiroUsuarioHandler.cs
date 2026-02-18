using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Empresas.CriarPrimeiroUsuario
{
    public class CriarPrimeiroUsuarioHandler : IRequestHandler<CriarPrimeiroUsuarioRequest, CriarPrimeiroUsuarioResponse>
    {
        private readonly AppDbContext _context;

        public CriarPrimeiroUsuarioHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CriarPrimeiroUsuarioResponse> Handle(CriarPrimeiroUsuarioRequest request, CancellationToken cancellationToken)
        {
            var empresa = await _context.Empresas.FindAsync(new object[] { request.EmpresaId }, cancellationToken);
            if (empresa == null)
            {
                return new CriarPrimeiroUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Empresa não encontrada."
                };
            }

            var emailNormalizado = request.Email.Trim().ToLower();
            var existe = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado, cancellationToken);
            if (existe)
            {
                return new CriarPrimeiroUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Já existe um usuário com este e-mail."
                };
            }

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            var usuario = new Usuario(
                request.EmpresaId,
                request.Nome.Trim(),
                request.Email.Trim(),
                emailNormalizado,
                senhaHash,
                "Administrador"
            )
            {
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            return new CriarPrimeiroUsuarioResponse
            {
                Sucesso = true,
                Mensagem = "Usuário criado com sucesso. O cliente pode fazer login com o e-mail e senha informados.",
                UsuarioId = usuario.Id
            };
        }
    }
}
