using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Estoque.GetListaMovimentacoes;

public class GetListaMovimentacoesHandler : IRequestHandler<GetListaMovimentacoesRequest, GetListaMovimentacoesResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaMovimentacoesHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaMovimentacoesResponse> Handle(GetListaMovimentacoesRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id.");

        var empresaId = _empresaContext.EmpresaId;

        var query = _context.MovimentacoesEstoque
            .Join(_context.Produtos,
                m => m.ProdutoId,
                p => p.Id,
                (m, p) => new { Movimentacao = m, Produto = p })
            .Where(x => x.Produto.EmpresaId == empresaId) // filtro por empresa via Produto
            .AsQueryable();

        if (request.ProdutoId.HasValue)
            query = query.Where(x => x.Movimentacao.ProdutoId == request.ProdutoId.Value);

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            var tipo = request.Tipo.Trim().ToLowerInvariant();
            query = query.Where(x => x.Movimentacao.TipoMovimentacao.ToLower().Contains(tipo));
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 100);

        var totalRegistros = await query.CountAsync(cancellationToken);
        var totalPaginas = totalRegistros > 0 ? (int)Math.Ceiling(totalRegistros / (double)tamanho) : 0;

        var itens = await query
            .OrderByDescending(x => x.Movimentacao.DataHora)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(x => new MovimentacaoResponse
            {
                Id = x.Movimentacao.Id,
                Tipo = x.Movimentacao.TipoMovimentacao.ToLower() == "entrada" ? "entrada" : "saida",
                ProdutoId = x.Movimentacao.ProdutoId,
                ProdutoNome = x.Produto.Nome,
                ProdutoSku = x.Produto.SKU,
                Quantidade = x.Movimentacao.Quantidade,
                Motivo = x.Movimentacao.Motivo,
                Observacoes = x.Movimentacao.Observacoes,
                Data = x.Movimentacao.DataHora
            })
            .ToListAsync(cancellationToken);

        return new GetListaMovimentacoesResponse
        {
            Movimentacoes = itens,
            PaginaAtual = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TamanhoPagina = tamanho
        };
    }
}
