using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasResponse
    {
        public List<CompraDto> Compras { get; set; } = new();
    }
}
