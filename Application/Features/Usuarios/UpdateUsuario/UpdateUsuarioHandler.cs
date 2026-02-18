using CRMVEXA.Application.Features.Usuarios.UpdateUsuario;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioRequest, UpdateUsuarioResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public UpdateUsuarioHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<UpdateUsuarioResponse> Handle(UpdateUsuarioRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var usuario = await _context.Usuarios
                .Where(u => u.EmpresaId == empresaId)
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
                var enderecosValidos = request.Enderecos
                    .Where(e => !string.IsNullOrWhiteSpace(e.Cep) || !string.IsNullOrWhiteSpace(e.Rua) ||
                                !string.IsNullOrWhiteSpace(e.Numero) || !string.IsNullOrWhiteSpace(e.Bairro) ||
                                !string.IsNullOrWhiteSpace(e.Cidade))
                    .ToList();
                foreach (var e in enderecosValidos)
                {
                    var endereco = new Endereco(
                        string.IsNullOrWhiteSpace(e.Rua) ? "" : e.Rua,
                        string.IsNullOrWhiteSpace(e.Numero) ? "" : e.Numero,
                        string.IsNullOrWhiteSpace(e.Bairro) ? "" : e.Bairro,
                        string.IsNullOrWhiteSpace(e.Cidade) ? "" : e.Cidade,
                        string.IsNullOrWhiteSpace(e.Cep) ? "" : e.Cep,
                        e.Complemento)
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
