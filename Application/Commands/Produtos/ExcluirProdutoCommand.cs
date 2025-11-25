using MediatR;

namespace CRMVEXA.Application.Commands.Produtos
{
    public record ExcluirProdutoCommand(
        int Id // Id do produto para excluir
    ) : IRequest<bool>;
}
