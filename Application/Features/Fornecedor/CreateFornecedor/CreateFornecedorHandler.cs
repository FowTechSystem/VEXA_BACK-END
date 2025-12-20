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
                    request.CategoriaProdutos,
                    request.CondicaoPagamento,
                    request.Observacoes,
                    request.DataCadastro
                );

            if (!string.IsNullOrWhiteSpace(request.Rua) || !string.IsNullOrWhiteSpace(request.Cep))
            {
                Endereco endereco = new Endereco
                (
                    request.Rua ?? string.Empty,
                    request.Numero ?? string.Empty,
                    request.Bairro ?? string.Empty,
                    request.Cidade ?? string.Empty,
                    request.Cep ?? string.Empty,
                    request.Complemento
                );

                fornecedor.AdicionarEndereco(endereco);
            }

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync(cancellationToken);

            return fornecedor.Id;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar Fornecedor: {ex.Message}", ex);
        }
    }


}

