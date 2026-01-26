using MediatR;

namespace CRMVEXA.Application.Features.Compras.DeleteCompra
{
    public class DeleteCompraRequest : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
