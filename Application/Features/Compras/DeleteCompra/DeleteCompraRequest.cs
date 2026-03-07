using MediatR;

namespace CRMVEXA.Application.Features.Compras.DeleteCompra
{
    public class DeleteCompraRequest : IRequest<DeleteCompraResponse>
    {
        public int Id { get; set; }
    }
}
