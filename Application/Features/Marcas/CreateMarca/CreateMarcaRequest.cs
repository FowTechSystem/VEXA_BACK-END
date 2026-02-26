using MediatR;

namespace CRMVEXA.Application.Features.Marcas.CreateMarca;

public class CreateMarcaRequest : IRequest<int>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
