using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Estoque.GetListaEstoque;

public class GetListaEstoqueHandler : IRequestHandler<GetListaEstoqueRequest, GetListaEstoqueResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaEstoqueHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaEstoqueResponse> Handle(GetListaEstoqueRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;

        var query = _context.Produtos
            .Where(p => p.EmpresaId == empresaId)
            .Include(p => p.Estoque)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Valor))
        {
            var valor = request.Valor.ToLower();
            query = query.Where(p => p.Nome.ToLower().Contains(valor) || (p.SKU != null && p.SKU.ToLower().Contains(valor)));
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 100);

        var totalRegistros = await query.CountAsync(cancellationToken);
        var totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling(totalRegistros / (double)tamanho) : 0;

        var produtos = await query
            .OrderBy(p => p.Nome)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        var itens = produtos.Select(p => new ProdutoEstoqueResponse
        {
            Id = p.Id,
            Nome = p.Nome,
            SKU = p.SKU ?? "",
            EstoqueAtual = p.Estoque?.Quantidade ?? 0,
            EstoqueMinimo = p.Estoque?.EstoqueMinimo ?? 1,
            EstoqueMaximo = 0,
            ValorUnitario = p.PrecoVenda
        }).ToList();

        return new GetListaEstoqueResponse
        {
            Produtos = itens,
            PaginaAtual = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TamanhoPagina = tamanho
        };
    }
}
