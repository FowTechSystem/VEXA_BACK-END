using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Clientes
{
    public record ObterClientePorIdQuery(int Id) : IRequest<ClienteDto>;
}
