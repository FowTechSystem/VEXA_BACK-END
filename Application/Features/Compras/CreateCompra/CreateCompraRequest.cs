using MediatR;

namespace CRMVEXA.Application.Features.Compras.CreateCompra
{
    public class CreateCompraRequest : IRequest<int>
    {
        public int FornecedorId { get; set; }
        public DateTime? DataPedido { get; set; }
        public DateTime? DataEntregaPrevista { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "pendente";
        public string? Observacoes { get; set; }
        public int? UsuarioId { get; set; }
        public List<ItemCompraRequest> Itens { get; set; } = new();
    }

    public class ItemCompraRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
