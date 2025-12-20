using CRMVEXA.Application.Features.Fornecedor.UpdateClienteFornecedor;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Fornecedor.UpdateFornecedor;

public class UpdateFornecedorHandler : IRequestHandler<UpdateFornecedorRequest, UpdateFornecedorResponse>
{
    private readonly AppDbContext _context;

    public UpdateFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateFornecedorResponse> Handle(UpdateFornecedorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fornecedor = await _context.Fornecedores
                .Include(f => f.Enderecos)
                .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

            if (fornecedor == null)
            {
                return new UpdateFornecedorResponse
                {
                    Sucesso = false,
                    Mensagem = "Fornecedor não encontrado."
                };
            }

            fornecedor.AtualizarDados(
                nome: request.Nome,
                razaoSocial: request.RazaoSocial,
                cnpj: request.CNPJ,
                nomeRepresentante: request.NomeRepresentante,
                telefone: request.Telefone,
                whatsApp: request.WhatsApp,
                email: request.Email,
                categoriaProdutos: request.CategoriaProdutos,
                condicaoPagamento: request.CondicaoPagamento,
                observacoes: request.Observacoes
            );

            var enderecoPrincipal = fornecedor.Enderecos.FirstOrDefault();

            if (enderecoPrincipal != null)
            {
                enderecoPrincipal.AtualizarEndereco(
                    rua: request.Rua ?? string.Empty,
                    numero: request.Numero ?? string.Empty,
                    bairro: request.Bairro ?? string.Empty,
                    cidade: request.Cidade ?? string.Empty,
                    cep: request.Cep ?? string.Empty,
                    complemento: request.Complemento
                );
            }
            else
            {
                var novoEndereco = new Endereco(
                    rua: request.Rua ?? string.Empty,
                    numero: request.Numero ?? string.Empty,
                    bairro: request.Bairro ?? string.Empty,
                    cidade: request.Cidade ?? string.Empty,
                    cep: request.Cep ?? string.Empty,
                    complemento: request.Complemento
                );

                fornecedor.AdicionarEndereco(novoEndereco);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateFornecedorResponse
            {
                Sucesso = true,
                Mensagem = "Fornecedor atualizado com sucesso!",
                Fornecedorid = fornecedor.Id
            };
        }
        catch (Exception ex)
        {
            // TODO: Adicionar notificação Discord aqui depois
            // await _discordService.EnviarErro(ex);

            return new UpdateFornecedorResponse
            {
                Sucesso = false,
                Mensagem = $"Erro ao atualizar fornecedor: {ex.Message}"
            };
        }
    }
}
