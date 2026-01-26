using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasResponse
    {
        public List<VendaDto> Vendas { get; set; } = new();
    }
}
