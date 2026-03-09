using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublico;

public class GetCatalogoPublicoRequest : IRequest<GetCatalogoPublicoResponse?>
{
    public int Id { get; set; }
}
