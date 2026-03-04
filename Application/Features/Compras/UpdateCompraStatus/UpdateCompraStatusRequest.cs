using MediatR;

namespace CRMVEXA.Application.Features.Compras.UpdateCompraStatus;

public class UpdateCompraStatusRequest : IRequest<UpdateCompraStatusResponse>
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateCompraStatusResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
