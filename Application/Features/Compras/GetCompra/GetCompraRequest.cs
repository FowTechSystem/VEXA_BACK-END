using MediatR;

namespace CRMVEXA.Application.Features.Compras.GetCompra
{
    public class GetCompraRequest : IRequest<GetCompraResponse>
    {
        public int Id { get; set; }
    }
}
