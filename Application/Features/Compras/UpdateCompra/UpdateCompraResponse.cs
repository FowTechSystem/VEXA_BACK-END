using CRMVEXA.Application.Features.Compras.GetCompra;
using CRMVEXA.Application.Features.Compras.GetListaCompras;

namespace CRMVEXA.Application.Features.Compras.UpdateCompra;

public class UpdateCompraResponse
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public GetCompraResponse? Compra { get; set; }
}
