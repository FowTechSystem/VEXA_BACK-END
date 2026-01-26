using System;
using CRMVEXA.Application.Features.Produtos.CreateProduto;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

namespace CRMVEXA.Application.Features.Produtos.CreateProduto
{
    public class CreateProdutoHandler : IRequestHandler<CreateProdutoRequest, int>
    {
        private readonly AppDbContext _context;

        public CreateProdutoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateProdutoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var produto = new Produto(
                    request.SKU,
                    request.Nome,
                    request.Categoria,
                    request.Genero,
                    request.PrecoCusto,
                    request.PrecoVenda)
                {
                    Subcategoria = request.Subcategoria,
                    Tamanho = request.Tamanho,
                    Cor = request.Cor,
                    Marca = request.Marca,
                    FornecedorId = request.FornecedorId,
                    CodigoBarras = request.CodigoBarras,
                    QuantidadeEstoque = request.QuantidadeEstoque,
                    EstoqueMinimo = request.EstoqueMinimo,
                    LocalizacaoEstoque = request.LocalizacaoEstoque,
                    Foto1 = request.Foto1,
                    Foto2 = request.Foto2,
                    Foto3 = request.Foto3,
                    Foto4 = request.Foto4,
                    Foto5 = request.Foto5,
                    DescricaoCurta = request.DescricaoCurta,
                    Peso = request.Peso
                };

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync(cancellationToken);

                return produto.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar produto: {ex.Message}", ex);
            }
        }
    }
}
