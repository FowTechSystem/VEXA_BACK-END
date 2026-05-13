using System;
using CRMVEXA.Application.Abstractions;
using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Application.Features.Usuarios.CriarUsuario;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Usuarios.CriarUsuario
{
    public class CriarUsuarioHandler : IRequestHandler<CriarUsuarioRequest, int>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;
        private readonly IUserTenantDirectoryService _directory;
        private readonly MasterDbContext _master;

        public CriarUsuarioHandler(
            AppDbContext context,
            IEmpresaContext empresaContext,
            IUserTenantDirectoryService directory,
            MasterDbContext master)
        {
            _context = context;
            _empresaContext = empresaContext;
            _directory = directory;
            _master = master;
        }

        public async Task<int> Handle(CriarUsuarioRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _empresaContext.EmpresaId;
                var emailKey = (request.Email ?? "").Trim().ToLowerInvariant();
                if (string.IsNullOrEmpty(emailKey))
                    throw new ArgumentException("E-mail é obrigatório.");

                if (await _master.UserTenantDirectory.AsNoTracking().AnyAsync(e => e.EmailNormalized == emailKey, cancellationToken).ConfigureAwait(false))
                    throw new Exception("Já existe um usuário com este e-mail em outra empresa ou neste catálogo.");

                if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailKey, cancellationToken).ConfigureAwait(false))
                    throw new Exception("Já existe um usuário com este e-mail.");

                var senhaTexto = !string.IsNullOrEmpty(request.SenhaHash) ? request.SenhaHash : request.Senha ?? "";
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaTexto);

                var usuario = new Usuario(
                    empresaId,
                    request.Nome,
                    request.Email,
                    request.Login,
                    senhaHash,
                    request.Cargo
                );

                usuario.CPF = request.CPF;
                usuario.CNPJ = request.CNPJ;
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

                usuario.Foto = ClienteFotoPayloadHelper.TryDecodeToBytes(request.FotoBase64);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync(cancellationToken);

                await _directory.UpsertAsync(empresaId, emailKey, usuario.Id, cancellationToken).ConfigureAwait(false);

                var enderecosValidos = request.Enderecos?
                    .Where(e => !string.IsNullOrWhiteSpace(e.Cep) || !string.IsNullOrWhiteSpace(e.Rua) || !string.IsNullOrWhiteSpace(e.Endereco) ||
                                !string.IsNullOrWhiteSpace(e.Numero) || !string.IsNullOrWhiteSpace(e.Bairro) ||
                                !string.IsNullOrWhiteSpace(e.Cidade))
                    .ToList() ?? new List<EnderecoRequest>();
                if (enderecosValidos.Count > 0)
                {
                    foreach (var e in enderecosValidos)
                    {
                        var rua = !string.IsNullOrWhiteSpace(e.Rua) ? e.Rua : (e.Endereco ?? "");
                        var endereco = new Endereco(
                            rua,
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
