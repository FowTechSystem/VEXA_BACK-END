using System;
using CRMVEXA.Application.Features.Produtos.GetProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoHandler : IRequestHandler<GetProdutoRequest, GetProdutoResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public GetProdutoHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<GetProdutoResponse> Handle(GetProdutoRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId)
                .Include(p => p.Categoria)
                .Include(p => p.Subcategoria)
                .Include(p => p.Genero)
                .Include(p => p.Tamanho)
                .Include(p => p.Cor)
                .Include(p => p.Marca)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            return new GetProdutoResponse
            {
                Id = produto.Id,
                SKU = produto.SKU,
                Nome = produto.Nome,
                Categoria = produto.Categoria?.Nome ?? string.Empty,
                Subcategoria = produto.Subcategoria?.Nome,
                Tamanho = produto.Tamanho?.Nome,
                Cor = produto.Cor?.Nome,
                Genero = produto.Genero?.Nome ?? string.Empty,
                Marca = produto.Marca?.Nome,
                FornecedorId = produto.FornecedorId,
                PrecoCusto = produto.PrecoCusto,
                PrecoVenda = produto.PrecoVenda,
                CodigoBarras = produto.CodigoBarras,
                QuantidadeEstoque = produto.Estoque?.Quantidade ?? 0,
                EstoqueMinimo = produto.Estoque?.EstoqueMinimo ?? 1,
                LocalizacaoEstoque = produto.Estoque?.LocalizacaoEstoque,
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
