using MediatR;

namespace CRMVEXA.Application.Commands.Vendas
{
    public record CancelarVendaCommand(
        int Id // Id da venda que será cancelada
    ) : IRequest<bool>;
}
