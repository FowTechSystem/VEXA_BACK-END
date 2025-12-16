using MediatR;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Features.Clientes.GetListaClientes;

public class GetListaClientesRequest : IRequest<GetListaClientesResponse>
{
    public string? Filtro { get; set; } // "nome" ou "cpf"
    public string? Valor { get; set; }
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 10;
}
