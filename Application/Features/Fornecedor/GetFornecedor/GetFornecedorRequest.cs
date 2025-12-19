using CRMVEXA.Application.Features.Clientes.GetListaClientes;
using MediatR;

namespace CRMVEXA.Application.Features.Fornecedor.GetFornecedor;

public class GetFornecedorRequest : IRequest<GetFornecedorResponse>
{
    public string? Filtro { get; set; }
    public string? Valor { get; set; }
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 10;
}
