using MediatR;

namespace CRMVEXA.Application.Commands.Clientes
{
    public record RemoverClienteCommand(
        int Id // Id do cliente para remover
    ) : IRequest<bool>;
}
