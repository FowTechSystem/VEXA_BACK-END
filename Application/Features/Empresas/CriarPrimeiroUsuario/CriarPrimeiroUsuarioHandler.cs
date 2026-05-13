using CRMVEXA.Application.Abstractions;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Empresas.CriarPrimeiroUsuario
{
    public class CriarPrimeiroUsuarioHandler : IRequestHandler<CriarPrimeiroUsuarioRequest, CriarPrimeiroUsuarioResponse>
    {
        private readonly ITenantAppDbContextFactory _dbFactory;
        private readonly IUserTenantDirectoryService _directory;
        private readonly MasterDbContext _master;

        public CriarPrimeiroUsuarioHandler(
            ITenantAppDbContextFactory dbFactory,
            IUserTenantDirectoryService directory,
            MasterDbContext master)
        {
            _dbFactory = dbFactory;
            _directory = directory;
            _master = master;
        }

        public async Task<CriarPrimeiroUsuarioResponse> Handle(CriarPrimeiroUsuarioRequest request, CancellationToken cancellationToken)
        {
            var tenantExists = await _master.Tenants.AsNoTracking()
                .AnyAsync(t => t.Id == request.EmpresaId && t.Ativo, cancellationToken)
                .ConfigureAwait(false);
            if (!tenantExists)
            {
                return new CriarPrimeiroUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Empresa não encontrada."
                };
            }

            var emailNormalizado = request.Email.Trim().ToLowerInvariant();

            var emailJaUsado = await _master.UserTenantDirectory.AsNoTracking()
                .AnyAsync(e => e.EmailNormalized == emailNormalizado, cancellationToken)
                .ConfigureAwait(false);
            if (emailJaUsado)
            {
                return new CriarPrimeiroUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Já existe um usuário com este e-mail."
                };
            }

            await using var context = _dbFactory.CreateDbContext(request.EmpresaId);

            var empresa = await context.Empresas.FindAsync(new object[] { request.EmpresaId }, cancellationToken).ConfigureAwait(false);
            if (empresa == null)
            {
                return new CriarPrimeiroUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Empresa não encontrada no banco do tenant."
                };
            }

            var existeNoTenant = await context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado, cancellationToken).ConfigureAwait(false);
            if (existeNoTenant)
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

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _directory.UpsertAsync(request.EmpresaId, usuario.Email.Trim().ToLowerInvariant(), usuario.Id, cancellationToken).ConfigureAwait(false);

            return new CriarPrimeiroUsuarioResponse
            {
                Sucesso = true,
                Mensagem = "Usuário criado com sucesso. O cliente pode fazer login com o e-mail e senha informados.",
                UsuarioId = usuario.Id
            };
        }
    }
}
