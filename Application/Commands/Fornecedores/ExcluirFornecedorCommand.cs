using MediatR;

namespace CRMVEXA.Application.Commands.Fornecedores
{
    public record ExcluirFornecedorCommand(
        int Id // Id do fornecedor para excluir
    ) : IRequest<bool>;
}
