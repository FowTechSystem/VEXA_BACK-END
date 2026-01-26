using MediatR;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoRequest : IRequest<GetProdutoResponse>
    {
        public int Id { get; set; }
    }
}
