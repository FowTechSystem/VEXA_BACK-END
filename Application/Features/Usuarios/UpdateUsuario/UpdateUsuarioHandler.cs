using CRMVEXA.Application.Features.Usuarios.UpdateUsuario;
using CRMVEXA.Domain.Entities;
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
                .Include(u => u.Enderecos)
                .Include(u => u.UsuarioPermissoes)
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
            usuario.CPF = request.CPF;
            usuario.RG = request.RG;
            usuario.Telefone = request.Telefone;
            usuario.WhatsApp = request.WhatsApp;
            usuario.DataNascimento = request.DataNascimento;
            usuario.Perfil = request.Perfil;
            usuario.Departamento = request.Departamento;
            usuario.Observacoes = request.Observacoes;
            usuario.Ativo = request.Ativo;

            if (!string.IsNullOrWhiteSpace(request.NovaSenha))
            {
                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            }

            usuario.DefinirPermissoes(
                request.PodeVender,
                request.PodeEditarEstoque,
                request.PodeEditarProduto,
                request.PodeAbrirFecharCaixa,
                request.PodeEmitirRelatorios,
                request.PodeCadastrarFornecedores
            );

            if (request.PermissaoIds != null)
            {
                _context.UsuarioPermissoes.RemoveRange(usuario.UsuarioPermissoes);
                foreach (var permissaoId in request.PermissaoIds)
                {
                    _context.UsuarioPermissoes.Add(new UsuarioPermissao
                    {
                        UsuarioId = usuario.Id,
                        PermissaoId = permissaoId
                    });
                }
            }

            if (request.Enderecos != null)
            {
                _context.Enderecos.RemoveRange(usuario.Enderecos);
                foreach (var e in request.Enderecos)
                {
                    var endereco = new Endereco(e.Rua, e.Numero, e.Bairro, e.Cidade, e.Cep, e.Complemento)
                    {
                        UsuarioId = usuario.Id,
                        Estado = e.Estado
                    };
                    _context.Enderecos.Add(endereco);
                }
            }

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
