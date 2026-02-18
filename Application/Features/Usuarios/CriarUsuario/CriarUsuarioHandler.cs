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
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(request.SenhaHash);

                var usuario = new Usuario(
                    request.Nome,
                    request.Email,
                    request.Login,
                    senhaHash,
                    request.Cargo
                );

                usuario.CPF = request.CPF;
                usuario.RG = request.RG;
                usuario.Telefone = request.Telefone;
                usuario.WhatsApp = request.WhatsApp;
                usuario.DataNascimento = request.DataNascimento;
                usuario.Perfil = request.Perfil;
                usuario.Departamento = request.Departamento;
                usuario.Observacoes = request.Observacoes;
                usuario.Ativo = request.Ativo;

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

                var enderecosValidos = request.Enderecos?
                    .Where(e => !string.IsNullOrWhiteSpace(e.Cep) || !string.IsNullOrWhiteSpace(e.Rua) ||
                                !string.IsNullOrWhiteSpace(e.Numero) || !string.IsNullOrWhiteSpace(e.Bairro) ||
                                !string.IsNullOrWhiteSpace(e.Cidade))
                    .ToList() ?? new List<EnderecoRequest>();
                if (enderecosValidos.Count > 0)
                {
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
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (request.PermissaoIds?.Count > 0)
                {
                    foreach (var permissaoId in request.PermissaoIds)
                    {
                        _context.UsuarioPermissoes.Add(new UsuarioPermissao
                        {
                            UsuarioId = usuario.Id,
                            PermissaoId = permissaoId
                        });
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return usuario.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar usuário: {ex.Message}", ex);
            }
        }
    }
}
