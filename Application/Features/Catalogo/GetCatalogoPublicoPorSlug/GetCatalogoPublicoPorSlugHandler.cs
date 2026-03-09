using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublicoPorSlug;

public class GetCatalogoPublicoPorSlugHandler : IRequestHandler<GetCatalogoPublicoPorSlugRequest, GetCatalogoPublicoPorSlugResponse?>
{
    private readonly AppDbContext _context;

    public GetCatalogoPublicoPorSlugHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetCatalogoPublicoPorSlugResponse?> Handle(GetCatalogoPublicoPorSlugRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
            return null;

        var catalogo = await _context.Catalogos
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Slug == request.Slug.Trim() && c.Ativo, cancellationToken);

        if (catalogo == null)
            return null;

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
