using MediatR;

namespace CRMVEXA.Application.Features.Clientes.DeleteCliente;

public class DeleteClienteRequest : IRequest<bool> 
{
    public int id { get; set; }
}
