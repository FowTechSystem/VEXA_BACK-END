using MediatR;

namespace CRMVEXA.Application.Features.Clientes.GetCliente;

public class GetClienteRequest : IRequest<GetClienteResponse?>
{
    public int Id { get; set; }
}
