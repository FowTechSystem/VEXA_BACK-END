using MediatR;

namespace CRMVEXA.Application.Features.Compras.UpdateCompra;

public class UpdateCompraRequest : IRequest<UpdateCompraResponse>
{
    public int Id { get; set; }
    public int FornecedorId { get; set; }
    public DateTime? DataPedido { get; set; }
    public DateTime? DataEntregaPrevista { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "pendente";
    public string? Observacoes { get; set; }
    public int? UsuarioId { get; set; }
    public List<UpdateCompraItemRequest> Itens { get; set; } = new();
}

public class UpdateCompraItemRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
