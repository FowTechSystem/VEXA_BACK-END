using MediatR;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasRequest : IRequest<GetListaComprasResponse>
    {
        public string? Valor { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 10;
    }
}
