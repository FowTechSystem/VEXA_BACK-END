using MediatR;

namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaRequest : IRequest<GetVendaResponse>
    {
        public int Id { get; set; }
    }
}
