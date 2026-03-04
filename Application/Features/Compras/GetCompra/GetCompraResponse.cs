using CRMVEXA.Application.Features.Compras.GetListaCompras;

namespace CRMVEXA.Application.Features.Compras.GetCompra
{
    public class GetCompraResponse
    {
        public int Id { get; set; }
        public string CodigoCompra { get; set; } = string.Empty;
        public int FornecedorId { get; set; }
        public FornecedorRefResponse? Fornecedor { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime DataPedido { get => DataHora; set => DataHora = value; }
        public DateTime? DataEntregaPrevista { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemCompraResponse> Itens { get; set; } = new();
    }

    public class ItemCompraResponse
    {
        public int Id { get; set; }
        public int CompraId { get; set; }
        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
