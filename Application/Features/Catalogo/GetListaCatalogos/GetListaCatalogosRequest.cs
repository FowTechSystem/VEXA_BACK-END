using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.GetListaCatalogos;

public class GetListaCatalogosRequest : IRequest<GetListaCatalogosResponse>
{
    public int Pagina { get; set; } = 1;
    public int Tamanho { get; set; } = 20;
}
