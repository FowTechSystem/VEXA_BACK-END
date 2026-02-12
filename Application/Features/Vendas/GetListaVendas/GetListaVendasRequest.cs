using MediatR;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasRequest : IRequest<GetListaVendasResponse>
    {
        public string? Valor { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 10;
    }
}
