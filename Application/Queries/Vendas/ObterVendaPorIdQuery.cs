using MediatR;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.Application.Queries.Vendas
{
    public record ObterVendaPorIdQuery(int Id) : IRequest<VendaDto>;
}
