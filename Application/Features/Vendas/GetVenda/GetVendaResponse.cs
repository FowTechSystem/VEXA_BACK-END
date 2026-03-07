namespace CRMVEXA.Application.Features.Vendas.GetVenda
{
    public class GetVendaResponse
    {
        public int Id { get; set; }
        public string CodigoVenda { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
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
        public List<ItemVendaResponse> Itens { get; set; } = new();
    }

    public class ItemVendaResponse
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public ProdutoRefResponse? Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ProdutoRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public decimal PrecoCusto { get; set; }
        public CategoriaRefResponse? Categoria { get; set; }
    }

    public class CategoriaRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class VendedorRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
