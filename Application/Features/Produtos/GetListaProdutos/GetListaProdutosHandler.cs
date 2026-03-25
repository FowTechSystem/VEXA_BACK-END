using CRMVEXA.Application.Features.Produtos.GetListaProdutos;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
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
            var query = _context.Produtos
                .Where(p => p.EmpresaId == empresaId)
                .Include(p => p.Categoria)
                .Include(p => p.Genero)
                .Include(p => p.Tamanho)
                .Include(p => p.Cor)
                .Include(p => p.Marca)
                .Include(p => p.Estoque)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Valor))
            {
                var valor = request.Valor.ToLower();
                if (request.Filtro?.ToLower() == "sku")
                    query = query.Where(p => p.SKU.ToLower().Contains(valor));
                else if (request.Filtro?.ToLower() == "categoria")
                    query = query.Where(p => p.Categoria != null && p.Categoria.Nome.ToLower().Contains(valor));
                else if (request.Filtro?.ToLower() == "marca")
                    query = query.Where(p => p.Marca != null && p.Marca.Nome.ToLower().Contains(valor));
                else
                    query = query.Where(p => p.Nome.ToLower().Contains(valor));
            }

            var pagina = Math.Max(1, request.Pagina);
            var tamanho = Math.Clamp(request.Tamanho, 1, 500);

            var totalRegistros = await query.CountAsync(cancellationToken);
            var totalPaginas = totalRegistros > 0
                ? (int)Math.Ceiling(totalRegistros / (double)tamanho)
                : 0;

            var produtos = await query
                .OrderBy(p => p.Nome)
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync(cancellationToken);

            var produtosResponse = produtos.Select(p => new ProdutoResponse
            {
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
                CodigoBarras = p.CodigoBarras,
                QuantidadeEstoque = p.Estoque?.Quantidade ?? 0,
                EstoqueMinimo = p.Estoque?.EstoqueMinimo ?? 10,
                EstoqueMaximo = 0,
                LocalizacaoEstoque = p.Estoque?.LocalizacaoEstoque,
                Foto1 = p.Foto1,
                Foto2 = p.Foto2,
                Foto3 = p.Foto3,
                Foto4 = p.Foto4,
                Foto5 = p.Foto5,
                DescricaoCurta = p.DescricaoCurta,
                Peso = p.Peso
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
    }
}
