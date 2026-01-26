using System;
using CRMVEXA.Application.Features.Produtos.UpdateProduto;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.UpdateProduto
{
    public class UpdateProdutoHandler : IRequestHandler<UpdateProdutoRequest, UpdateProdutoResponse>
    {
        private readonly AppDbContext _context;

        public UpdateProdutoHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateProdutoResponse> Handle(UpdateProdutoRequest request, CancellationToken cancellationToken)
        {
            var produto = await _context.Produtos.FindAsync(new object[] { request.Id }, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            produto.AtualizarDados(
                request.SKU,
                request.Nome,
                request.Categoria,
                request.Genero,
                request.PrecoCusto,
                request.PrecoVenda,
                request.Subcategoria,
                request.Tamanho,
                request.Cor,
                request.Marca,
                request.FornecedorId,
                request.CodigoBarras,
                request.EstoqueMinimo,
                request.LocalizacaoEstoque,
                request.Foto1,
                request.Foto2,
                request.Foto3,
                request.Foto4,
                request.Foto5,
                request.DescricaoCurta,
                request.Peso
            );

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateProdutoResponse { Id = produto.Id };
        }
    }
}
