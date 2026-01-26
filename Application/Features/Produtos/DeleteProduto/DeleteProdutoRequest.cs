using MediatR;

namespace CRMVEXA.Application.Features.Produtos.DeleteProduto
{
    public class DeleteProdutoRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
