using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using CRMVEXA.Domain.Entities;
using MediatR;

namespace CRMVEXA.Application.Features.Fornecedor.CreateFornecedor;

public class CreateFornecedorHandler : IRequestHandler<CreateFornecedorRequest, int>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public CreateFornecedorHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<int> Handle(CreateFornecedorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fornecedor = new CRMVEXA.Domain.Entities.Fornecedor
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
            fornecedor.EmpresaId = _empresaContext.EmpresaId;

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

