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
        public DateTime DataHora { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemVendaResponse> Itens { get; set; } = new();
    }

    public class ItemVendaResponse
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
