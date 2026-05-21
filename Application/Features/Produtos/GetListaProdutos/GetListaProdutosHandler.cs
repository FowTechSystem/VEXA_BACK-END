using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Application.Services;
using CRMVEXA.Application.Services.Bazar;
using CRMVEXA.Domain.Enums;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos;

public class GetListaProdutosHandler : IRequestHandler<GetListaProdutosRequest, GetListaProdutosResponse>
{
    private readonly AppDbContext _context;
    private readonly IEmpresaContext _empresaContext;

    public GetListaProdutosHandler(AppDbContext context, IEmpresaContext empresaContext)
    {
        _context = context;
        _empresaContext = empresaContext;
    }

    public async Task<GetListaProdutosResponse> Handle(GetListaProdutosRequest request, CancellationToken cancellationToken)
    {
        if (!_empresaContext.TemEmpresa)
            throw new InvalidOperationException("EmpresaId não disponível. Envie o token JWT e o header X-Empresa-Id (ex: X-Empresa-Id: 1) para dev, ou use um token com claim empresa_id.");

        var empresaId = _empresaContext.EmpresaId;
        var filtro = request.Filtro?.ToLowerInvariant();
        var incluirBazar = filtro is not ("sku" or "categoria" or "marca");
        var termoBusca = request.Valor?.Trim().ToLowerInvariant() ?? "";

        var query = _context.Produtos
            .AsNoTracking()
            .Where(p => p.EmpresaId == empresaId);

        if (!string.IsNullOrEmpty(termoBusca))
        {
            query = filtro switch
            {
                "sku" => query.Where(p => p.SKU.ToLower().Contains(termoBusca)),
                "categoria" => query.Where(p => p.Categoria != null && p.Categoria.Nome.ToLower().Contains(termoBusca)),
                "marca" => query.Where(p => p.Marca != null && p.Marca.Nome.ToLower().Contains(termoBusca)),
                _ => query.Where(p =>
                    p.Nome.ToLower().Contains(termoBusca) ||
                    (p.SKU != null && p.SKU.ToLower().Contains(termoBusca)) ||
                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(termoBusca)))
            };
        }

        var produtosLight = await query
            .Select(p => new ItemListaUnificada { Tipo = "normal", Id = p.Id, Nome = p.Nome })
            .ToListAsync(cancellationToken);

        var bazarLight = new List<ItemListaUnificada>();
        if (incluirBazar)
        {
            var bazarQuery = _context.BazarProdutos.AsNoTracking()
                .Where(p => p.EmpresaId == empresaId && p.DeletadoEm == null && p.Status == BazarProdutoStatus.Disponivel);

            if (!string.IsNullOrEmpty(termoBusca))
            {
                bazarQuery = bazarQuery.Where(p =>
                    p.Descricao.ToLower().Contains(termoBusca) ||
                    (p.CodigoEtiqueta != null && p.CodigoEtiqueta.ToLower().Contains(termoBusca)) ||
                    _context.BazarConsignantes.Any(c =>
                        c.Id == p.ConsignanteId && c.DeletadoEm == null && c.Nome.ToLower().Contains(termoBusca)));
            }

            bazarLight = await bazarQuery
                .Select(p => new ItemListaUnificada { Tipo = "bazar", Id = p.Id, Nome = p.Descricao })
                .ToListAsync(cancellationToken);
        }

        var pagina = Math.Max(1, request.Pagina);
        var tamanho = Math.Clamp(request.Tamanho, 1, 500);

        var merged = produtosLight
            .Concat(bazarLight)
            .OrderBy(x => x.Nome, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var totalRegistros = merged.Count;
        var totalPaginas = totalRegistros > 0
            ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
            : 0;

        var paginaItens = merged
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToList();

        var produtoIds = paginaItens.Where(x => x.Tipo == "normal").Select(x => x.Id).ToList();
        var bazarIds = paginaItens.Where(x => x.Tipo == "bazar").Select(x => x.Id).ToList();

        var produtosDb = produtoIds.Count > 0
            ? await _context.Produtos.AsNoTracking()
                .Where(p => produtoIds.Contains(p.Id))
                .Include(p => p.Categoria)
                .Include(p => p.Genero)
                .Include(p => p.Tamanho)
                .Include(p => p.Cor)
                .Include(p => p.Marca)
                .Include(p => p.Estoque)
                .ToDictionaryAsync(p => p.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.Produto>();

        var bazarDb = bazarIds.Count > 0
            ? await _context.BazarProdutos.AsNoTracking()
                .Include(p => p.Consignante)
                .Include(p => p.Fotos)
                .Where(p => bazarIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken)
            : new Dictionary<int, Domain.Entities.BazarProduto>();

        var produtosResponse = paginaItens.Select(item =>
        {
            if (item.Tipo == "bazar" && bazarDb.TryGetValue(item.Id, out var peca))
                return MapBazar(peca);
            if (item.Tipo == "normal" && produtosDb.TryGetValue(item.Id, out var p))
                return MapNormal(p);
            return new ProdutoResponse { Id = item.Id, Nome = item.Nome, Tipo = item.Tipo };
        }).ToList();

        return new GetListaProdutosResponse
        {
            Produtos = produtosResponse,
            PaginaAtual = pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            TamanhoPagina = tamanho
        };
    }

    private static ProdutoResponse MapNormal(Domain.Entities.Produto p) => new()
    {
        Tipo = "normal",
        Id = p.Id,
        SKU = p.SKU,
        Nome = p.Nome,
        CategoriaId = p.CategoriaId,
        Categoria = p.Categoria != null ? new CategoriaRefResponse { Id = p.Categoria.Id, Nome = p.Categoria.Nome } : null,
        CategoriaNome = p.Categoria?.Nome,
        Subcategoria = null,
        Tamanho = p.Tamanho?.Nome,
        Cor = p.Cor?.Nome,
        Genero = p.Genero?.Nome ?? string.Empty,
        MarcaId = p.MarcaId,
        Marca = p.Marca != null ? new MarcaRefResponse { Id = p.Marca.Id, Nome = p.Marca.Nome } : null,
        FornecedorId = p.FornecedorId,
        PrecoCusto = p.PrecoCusto,
        PrecoVenda = p.PrecoVenda,
        PrecoPromocional = p.PrecoPromocional,
        EmPromocao = p.EmPromocao,
        Observacoes = p.Observacoes,
        CodigoBarras = p.CodigoBarras,
        QuantidadeEstoque = p.Estoque?.Quantidade ?? 0,
        EstoqueMinimo = p.Estoque?.EstoqueMinimo ?? 10,
        EstoqueMaximo = 0,
        LocalizacaoEstoque = p.Estoque?.LocalizacaoEstoque,
        Foto1 = ClienteFotoUrlHelper.ToApiFotoUrl(p.Foto1),
        Foto2 = ClienteFotoUrlHelper.ToApiFotoUrl(p.Foto2),
        Foto3 = ClienteFotoUrlHelper.ToApiFotoUrl(p.Foto3),
        Foto4 = ClienteFotoUrlHelper.ToApiFotoUrl(p.Foto4),
        Foto5 = ClienteFotoUrlHelper.ToApiFotoUrl(p.Foto5),
        DescricaoCurta = p.DescricaoCurta,
        Peso = p.Peso,
        Cores = p.Cores.Count > 0 ? p.Cores.ToList() : new List<string>(),
        Tamanhos = p.Tamanhos.Count > 0 ? p.Tamanhos.ToList() : new List<string>(),
        TipoTamanho = p.TipoTamanho,
        Material = p.Material
    };

    private static ProdutoResponse MapBazar(Domain.Entities.BazarProduto peca)
    {
        var foto = peca.Fotos.OrderBy(f => f.Ordem).FirstOrDefault();
        return new ProdutoResponse
        {
            Tipo = "bazar",
            Id = peca.Id,
            SKU = "BAZAR",
            Nome = peca.Descricao,
            ConsignanteNome = peca.Consignante?.Nome,
            StatusBazar = BazarStatusHelper.ToApi(peca.Status),
            Tamanho = peca.Tamanho,
            Cor = peca.Cor,
            PrecoCusto = peca.ValorCompra,
            PrecoVenda = peca.ValorVenda,
            CodigoBarras = peca.CodigoEtiqueta,
            QuantidadeEstoque = 1,
            EstoqueMinimo = 0,
            EstoqueMaximo = 1,
            CategoriaNome = "Bazar",
            Foto1 = foto?.Url,
            Observacoes = peca.SacolaId > 0 ? $"Sacola #{peca.SacolaId}" : null
        };
    }

    private sealed class ItemListaUnificada
    {
        public string Tipo { get; set; } = "normal";
        public int Id { get; set; }
        public string Nome { get; set; } = "";
    }
}
