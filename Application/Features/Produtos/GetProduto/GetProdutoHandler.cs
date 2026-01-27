using System;
using CRMVEXA.Application.Features.Produtos.GetProduto;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoHandler : IRequestHandler<GetProdutoRequest, GetProdutoResponse>
    {
        private readonly AppDbContext _context;

        public GetProdutoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetProdutoResponse> Handle(GetProdutoRequest request, CancellationToken cancellationToken)
        {
            var produto = await _context.Produtos.FindAsync(new object[] { request.Id }, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            return new GetProdutoResponse
            {
                Id = produto.Id,
                SKU = produto.SKU,
                Nome = produto.Nome,
                Categoria = produto.Categoria,
                Subcategoria = produto.Subcategoria,
                Tamanho = produto.Tamanho,
                Cor = produto.Cor,
                Genero = produto.Genero,
                Marca = produto.Marca,
                FornecedorId = produto.FornecedorId,
                PrecoCusto = produto.PrecoCusto,
                PrecoVenda = produto.PrecoVenda,
                CodigoBarras = produto.CodigoBarras,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                EstoqueMinimo = produto.EstoqueMinimo,
                LocalizacaoEstoque = produto.LocalizacaoEstoque,
                Foto1 = produto.Foto1,
                Foto2 = produto.Foto2,
                Foto3 = produto.Foto3,
                Foto4 = produto.Foto4,
                Foto5 = produto.Foto5,
                DescricaoCurta = produto.DescricaoCurta,
                Peso = produto.Peso
            };
        }
    }
}
