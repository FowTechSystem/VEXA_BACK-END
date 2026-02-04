using CRMVEXA.Application.Features.Usuarios.UpdateUsuario;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioRequest, UpdateUsuarioResponse>
    {
        private readonly AppDbContext _context;

        public UpdateUsuarioHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateUsuarioResponse> Handle(UpdateUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (usuario == null)
            {
                return new UpdateUsuarioResponse
                {
                    Sucesso = false,
                    Mensagem = "Usuário não encontrado."
                };
            }

            usuario.Nome = request.Nome;
            usuario.Email = request.Email;
            usuario.Login = request.Login;
            usuario.Cargo = request.Cargo;
            usuario.DefinirPermissoes(
                request.PodeVender,
                request.PodeEditarEstoque,
                request.PodeEditarProduto,
                request.PodeAbrirFecharCaixa,
                request.PodeEmitirRelatorios,
                request.PodeCadastrarFornecedores
            );

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateUsuarioResponse
            {
                Sucesso = true,
                Mensagem = "Usuário atualizado com sucesso!",
                UsuarioId = usuario.Id
            };
        }
    }
}
