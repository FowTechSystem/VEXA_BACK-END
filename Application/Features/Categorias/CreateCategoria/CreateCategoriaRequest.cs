using MediatR;

namespace CRMVEXA.Application.Features.Categorias.CreateCategoria;

public class CreateCategoriaRequest : IRequest<int>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
