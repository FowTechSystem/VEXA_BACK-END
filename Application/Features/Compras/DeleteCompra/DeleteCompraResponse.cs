namespace CRMVEXA.Application.Features.Compras.DeleteCompra;

public class DeleteCompraResponse
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public int Codigo { get; set; } // 404, 409
}
