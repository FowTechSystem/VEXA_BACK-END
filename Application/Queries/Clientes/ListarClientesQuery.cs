using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Clientes
{
    public record ListarClientesQuery() : IRequest<IEnumerable<ClienteDto>>;
}
