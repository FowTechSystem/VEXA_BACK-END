using CRMVEXA.Application.Features.Compras.GetCompra;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasResponse
    {
        public List<CompraResponse> Compras { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanhoPagina { get; set; }
    }

    public class CompraResponse
    {
        public int Id { get; set; }
        public string CodigoCompra { get; set; } = string.Empty;
        public int FornecedorId { get; set; }
        public FornecedorRefResponse? Fornecedor { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public DateTime DataHora { get; set; }
        /// <summary>Alias para DataHora (data do pedido).</summary>
        public DateTime DataPedido { get => DataHora; set => DataHora = value; }
        public DateTime? DataEntregaPrevista { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemCompraResponse> Itens { get; set; } = new();
    }

    public class FornecedorRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? RazaoSocial { get; set; }
    }
}
