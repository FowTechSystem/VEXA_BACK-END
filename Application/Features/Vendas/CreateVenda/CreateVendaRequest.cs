using MediatR;

namespace CRMVEXA.Application.Features.Vendas.CreateVenda
{
    public class CreateVendaRequest : IRequest<int>
    {
        public string CodigoVenda { get; set; } = string.Empty;
        public int? ClienteId { get; set; }
        public int UsuarioId { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public decimal Desconto { get; set; }
        public List<ItemVendaRequest> Itens { get; set; } = new();
    }

    public class ItemVendaRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
