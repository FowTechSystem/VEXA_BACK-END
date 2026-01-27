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
            var produtos = await _context.Produtos.ToListAsync(cancellationToken);
            
            var produtosResponse = produtos.Select(p => new ProdutoResponse
            {
                Id = p.Id,
                SKU = p.SKU,
                Nome = p.Nome,
                Categoria = p.Categoria,
                Subcategoria = p.Subcategoria,
                Tamanho = p.Tamanho,
                Cor = p.Cor,
                Genero = p.Genero,
                Marca = p.Marca,
                FornecedorId = p.FornecedorId,
                PrecoCusto = p.PrecoCusto,
                PrecoVenda = p.PrecoVenda,
                CodigoBarras = p.CodigoBarras,
                QuantidadeEstoque = p.QuantidadeEstoque,
                EstoqueMinimo = p.EstoqueMinimo,
                LocalizacaoEstoque = p.LocalizacaoEstoque,
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
