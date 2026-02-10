using CRMVEXA.Application.Features.Produtos.GetListaProdutos;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
    public class GetListaProdutosHandler : IRequestHandler<GetListaProdutosRequest, GetListaProdutosResponse>
    {
        private readonly AppDbContext _context;

        public GetListaProdutosHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetListaProdutosResponse> Handle(GetListaProdutosRequest request, CancellationToken cancellationToken)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Subcategoria)
                .Include(p => p.Genero)
                .Include(p => p.Tamanho)
                .Include(p => p.Cor)
                .Include(p => p.Marca)
                .Include(p => p.Estoque)
                .ToListAsync(cancellationToken);

            var produtosResponse = produtos.Select(p => new ProdutoResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Nome = p.Nome,
                Categoria = p.Categoria?.Nome ?? string.Empty,
                Subcategoria = p.Subcategoria?.Nome,
                Tamanho = p.Tamanho?.Nome,
                Cor = p.Cor?.Nome,
                Genero = p.Genero?.Nome ?? string.Empty,
                Marca = p.Marca?.Nome,
                FornecedorId = p.FornecedorId,
                PrecoCusto = p.PrecoCusto,
                PrecoVenda = p.PrecoVenda,
                CodigoBarras = p.CodigoBarras,
                QuantidadeEstoque = p.Estoque?.Quantidade ?? 0,
                EstoqueMinimo = p.Estoque?.EstoqueMinimo ?? 1,
                LocalizacaoEstoque = p.Estoque?.LocalizacaoEstoque,
                Foto1 = p.Foto1,
                Foto2 = p.Foto2,
                Foto3 = p.Foto3,
                Foto4 = p.Foto4,
                Foto5 = p.Foto5,
                DescricaoCurta = p.DescricaoCurta,
                Peso = p.Peso
            }).ToList();

            return new GetListaProdutosResponse { Produtos = produtosResponse };
        }
    }
}
