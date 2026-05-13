using CRMVEXA.Infrastructure.MultiTenancy;
using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublico;

public class GetCatalogoPublicoHandler : IRequestHandler<GetCatalogoPublicoRequest, GetCatalogoPublicoResponse?>
{
    private readonly ICrossTenantResolver _crossTenant;

    public GetCatalogoPublicoHandler(ICrossTenantResolver crossTenant)
    {
        _crossTenant = crossTenant;
    }

    public async Task<GetCatalogoPublicoResponse?> Handle(GetCatalogoPublicoRequest request, CancellationToken cancellationToken)
    {
        var found = await _crossTenant.FindCatalogoByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
        if (found == null)
            return null;

        var catalogo = found.Catalogo;

        return new GetCatalogoPublicoResponse
        {
            Id = catalogo.Id,
            EmpresaNome = catalogo.EmpresaNome,
            CriadorWhatsApp = catalogo.CriadorWhatsApp,
            Produtos = catalogo.Produtos.Select(p => new GetCatalogoPublicoItemResponse
            {
                ProdutoId = p.ProdutoId,
                Nome = p.Nome,
                Preco = p.Preco,
                Quantidade = p.Quantidade
            }).ToList()
        };
    }
}
