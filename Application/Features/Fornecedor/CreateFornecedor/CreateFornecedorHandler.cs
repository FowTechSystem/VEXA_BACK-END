using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Domain.Entities;
using MediatR;

namespace CRMVEXA.Application.Features.Fornecedor.CreateFornecedor;

public class CreateFornecedorHandler : IRequestHandler<CreateFornecedorRequest, int>
{
    private readonly AppDbContext _context;

    public CreateFornecedorHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateFornecedorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            CRMVEXA.Domain.Entities.Fornecedor fornecedor = 
                new CRMVEXA.Domain.Entities.Fornecedor
                (
                    request.Nome,
                    request.RazaoSocial,
                    request.CNPJ,
                    request.NomeRepresentante,
                    request.Telefone,
                    request.WhatsApp,
                    request.Email,
                    request.Endereco,
                    request.CategoriaProdutos,
                    request.CondicaoPagamento,
                    request.Observacoes,
                    request.DataCadastro
                );

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync(cancellationToken);

            return fornecedor.Id;
        }
        catch (Exception ex)
        {
            //TODO: Adicionar notificação Discord aqui depois
            //await _discordService.EnviarErro(ex);

            throw new Exception($"Erro ao criar Fornecedor: {ex.Message}", ex);
        }
    }
}

