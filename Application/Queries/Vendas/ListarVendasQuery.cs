using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Vendas
{
    public record ListarVendasQuery() : IRequest<IEnumerable<VendaDto>>;
}
