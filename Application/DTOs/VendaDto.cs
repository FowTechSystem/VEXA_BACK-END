namespace CRMVEXA.Application.DTOs
{
    public class VendaDto
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
        public List<ItemVendaDto> Itens { get; set; } = new();
    }

    public class ItemVendaDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
