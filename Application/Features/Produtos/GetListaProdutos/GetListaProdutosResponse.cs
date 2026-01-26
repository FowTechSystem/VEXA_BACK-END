using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
    public class GetListaProdutosResponse
    {
        public List<ProdutoDto> Produtos { get; set; } = new();
    }
}
