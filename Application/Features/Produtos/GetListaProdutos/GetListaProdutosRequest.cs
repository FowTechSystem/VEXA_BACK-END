using MediatR;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
    public class GetListaProdutosRequest : IRequest<GetListaProdutosResponse>
    {
        public string? Filtro { get; set; }
        public string? Valor { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 10;
    }
}
