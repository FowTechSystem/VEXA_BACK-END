using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublicoPorSlug;

public class GetCatalogoPublicoPorSlugHandler : IRequestHandler<GetCatalogoPublicoPorSlugRequest, GetCatalogoPublicoPorSlugResponse?>
{
    private readonly ICrossTenantResolver _crossTenant;

    public GetCatalogoPublicoPorSlugHandler(ICrossTenantResolver crossTenant)
    {
        _crossTenant = crossTenant;
    }

    public async Task<GetCatalogoPublicoPorSlugResponse?> Handle(GetCatalogoPublicoPorSlugRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
            return null;

        var found = await _crossTenant.FindCatalogoBySlugAsync(request.Slug, cancellationToken).ConfigureAwait(false);
        if (found == null)
            return null;

        var catalogo = found.Catalogo;

        return new GetCatalogoPublicoPorSlugResponse
        {
            Id = catalogo.Id,
            EmpresaNome = catalogo.EmpresaNome,
            CriadorWhatsApp = catalogo.CriadorWhatsApp,
            Produtos = catalogo.Produtos.Select(p => new GetCatalogoPublicoPorSlugItemResponse
            {
                ProdutoId = p.ProdutoId,
                Nome = p.Nome,
                Preco = p.Preco,
                Quantidade = p.Quantidade
            }).ToList()
        };
    }
}
