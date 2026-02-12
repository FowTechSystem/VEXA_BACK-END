using CRMVEXA.Application.Features.Vendas.GetVenda;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasResponse
    {
        public List<VendaResponse> Vendas { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanhoPagina { get; set; }
    }

    public class VendaResponse
    {
        public int Id { get; set; }
        public string CodigoVenda { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
        public decimal Desconto { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemVendaResponse> Itens { get; set; } = new();
    }
}
