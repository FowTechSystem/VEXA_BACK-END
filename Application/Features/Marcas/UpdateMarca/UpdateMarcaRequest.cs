using MediatR;

namespace CRMVEXA.Application.Features.Marcas.UpdateMarca;

public class UpdateMarcaRequest : IRequest<UpdateMarcaResponse>
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UpdateMarcaResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
