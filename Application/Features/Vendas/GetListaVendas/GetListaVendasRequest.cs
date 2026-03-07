using MediatR;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    /// <summary>
    /// Request para GET /api/vendas (spec Caixa - Histórico de Vendas).
    /// dataInicio/dataFim (ISO) ou mes/ano para filtrar por período.
    /// </summary>
    public class GetListaVendasRequest : IRequest<GetListaVendasResponse>
    {
        public string? Valor { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? Mes { get; set; }
        public int? Ano { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 500;
    }
}
