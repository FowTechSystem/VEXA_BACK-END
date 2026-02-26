using MediatR;

namespace CRMVEXA.Application.Features.Marcas.DeleteMarca;

public class DeleteMarcaRequest : IRequest<DeleteMarcaResponse>
{
    public int Id { get; set; }
}

public class DeleteMarcaResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
