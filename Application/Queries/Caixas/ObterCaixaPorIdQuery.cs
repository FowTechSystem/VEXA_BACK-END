using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Caixas
{
    public record ObterCaixaPorIdQuery(int Id) : IRequest<CaixaDto>;
}
