using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Features.Clientes.GetListaClientes;

public class GetListaClientesResponse
{
    public List<Cliente> Clientes { get; set; } = new();
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }
    public int TamanhoPagina { get; set; }
}
