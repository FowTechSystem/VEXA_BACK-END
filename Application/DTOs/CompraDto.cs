namespace CRMVEXA.Application.DTOs
{
    public class CompraDto
    {
        public int Id { get; set; }
        public string CodigoCompra { get; set; } = string.Empty;
        public int FornecedorId { get; set; }
        public string? FornecedorNome { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public DateTime DataHora { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<ItemCompraDto> Itens { get; set; } = new();
    }

    public class ItemCompraDto
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
