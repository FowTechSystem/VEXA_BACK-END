using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublico;

public class GetCatalogoPublicoHandler : IRequestHandler<GetCatalogoPublicoRequest, GetCatalogoPublicoResponse?>
{
    private readonly AppDbContext _context;

    public GetCatalogoPublicoHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetCatalogoPublicoResponse?> Handle(GetCatalogoPublicoRequest request, CancellationToken cancellationToken)
    {
        var catalogo = await _context.Catalogos
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.Ativo, cancellationToken);

        if (catalogo == null)
            return null;

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
