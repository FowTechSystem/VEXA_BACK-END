using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublicoPorSlug;

public class GetCatalogoPublicoPorSlugRequest : IRequest<GetCatalogoPublicoPorSlugResponse?>
{
    public string Slug { get; set; } = string.Empty;
}
