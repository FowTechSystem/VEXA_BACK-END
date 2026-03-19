using CRMVEXA.Application.Features.Vendas.GetVenda;

namespace CRMVEXA.Application.Features.Vendas.GetListaVendas
{
    public class GetListaVendasResponse
    {
        public List<VendaResponse> Vendas { get; set; } = new();
        public int Total => TotalRegistros;
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
        /// <summary>Objeto cliente para relatórios (cliente.nome).</summary>
        public ClienteRefResponse? Cliente { get; set; }
        public string? NomeCliente { get; set; }
        public decimal Desconto { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public int VendedorId { get => UsuarioId; set => UsuarioId = value; }
        public VendedorRefResponse? Vendedor { get; set; }
        public VendedorRefResponse? Usuario { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime DataVenda { get => DataHora; set => DataHora = value; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public List<ItemVendaResponse> Itens { get; set; } = new();
    }

    public class VendedorRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class ClienteRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
