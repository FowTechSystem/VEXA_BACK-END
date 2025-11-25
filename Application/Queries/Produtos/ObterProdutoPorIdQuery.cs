using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Produtos
{
    public record ObterProdutoPorIdQuery(int Id) : IRequest<ProdutoDto>;
}
