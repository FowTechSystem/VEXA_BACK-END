using System;
using CRMVEXA.Application.Features.Usuarios.CriarUsuario;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Usuarios.CriarUsuario
{
    public class CriarUsuarioHandler : IRequestHandler<CriarUsuarioRequest, int>
    {
        private readonly AppDbContext _context;

        public CriarUsuarioHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CriarUsuarioRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = new Usuario(
                    request.Nome,
                    request.Email,
                    request.Login,
                    request.SenhaHash,
                    request.Cargo
                );
                usuario.DefinirPermissoes(
                    request.PodeVender,
                    request.PodeEditarEstoque,
                    request.PodeEditarProduto,
                    request.PodeAbrirFecharCaixa,
                    request.PodeEmitirRelatorios,
                    request.PodeCadastrarFornecedores
                );

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync(cancellationToken);

                return usuario.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar usuário: {ex.Message}", ex);
            }
        }
    }
}
