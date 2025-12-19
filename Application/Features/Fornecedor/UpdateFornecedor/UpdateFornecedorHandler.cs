using CRMVEXA.Application.Features.Fornecedor.UpdateClienteFornecedor;
using CRMVEXA.Infrastructure.Data;
using MediatR;

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
            var fornecedor = await _context.Fornecedores.FindAsync(new object?[] { request.Id }, cancellationToken);

            if (fornecedor == null)
            {
                throw new Exception("Fornecedor não encontrado.");
            }

            fornecedor.AtualizarDados(
                nome: request.Nome,
                razaoSocial: request.RazaoSocial,
                cnpj: request.CNPJ,
                nomeRepresentante: request.NomeRepresentante,
                telefone: request.Telefone,
                whatsApp: request.WhatsApp,
                email: request.Email,
                endereco: request.Endereco,
                categoriaProdutos: request.CategoriaProdutos,
                condicaoPagamento: request.CondicaoPagamento,
                observacoes: request.Observacoes,
                dataCadastro: request.DataCadastro
            );

            await _context.SaveChangesAsync(cancellationToken);
            return new UpdateFornecedorResponse
            {
                Sucesso = true,
                Mensagem = "Cliente atualizado com sucesso!",
                Fornecedorid = fornecedor.Id
            };
        }
        catch (Exception ex)
        {
            // TODO: Adicionar notificação Discord aqui depois
            // await _discordService.EnviarErro(ex);

            throw new Exception($"Erro ao editar Fornecedor: {ex.Message}", ex);
        }

    }
}
