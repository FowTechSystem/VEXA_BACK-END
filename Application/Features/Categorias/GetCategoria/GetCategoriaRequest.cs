using MediatR;

namespace CRMVEXA.Application.Features.Categorias.GetCategoria;

public class GetCategoriaRequest : IRequest<GetCategoriaResponse?>
{
    public int Id { get; set; }
}
