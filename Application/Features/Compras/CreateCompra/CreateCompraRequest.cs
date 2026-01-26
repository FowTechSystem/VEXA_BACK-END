using MediatR;

namespace CRMVEXA.Application.Features.Compras.CreateCompra
{
    public class CreateCompraRequest : IRequest<int>
    {
        public string CodigoCompra { get; set; } = string.Empty;
        public int FornecedorId { get; set; }
        public int UsuarioId { get; set; }
        public string? Observacoes { get; set; }
        public List<ItemCompraRequest> Itens { get; set; } = new();
    }

    public class ItemCompraRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
