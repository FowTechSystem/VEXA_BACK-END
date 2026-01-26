using MediatR;

namespace CRMVEXA.Application.Features.Vendas.CancelarVenda
{
    public class CancelarVendaRequest : IRequest<Unit>
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
    }
}
