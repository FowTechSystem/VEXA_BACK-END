using MediatR;
using System.Collections.Generic;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Caixas
{
    public record ListarCaixasQuery() : IRequest<IEnumerable<CaixaDto>>;
}
