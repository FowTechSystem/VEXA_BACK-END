using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoResponse
    {
        public ProdutoDto Produto { get; set; } = new();
    }
}
