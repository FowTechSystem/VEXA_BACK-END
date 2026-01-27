using CRMVEXA.Application.Features.Compras.GetCompra;

namespace CRMVEXA.Application.Features.Compras.GetListaCompras
{
    public class GetListaComprasResponse
    {
        public List<CompraResponse> Compras { get; set; } = new();
    }

    public class CompraResponse
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
        public List<ItemCompraResponse> Itens { get; set; } = new();
    }
}
