using System;
using CRMVEXA.Application.Features.Produtos.UpdateProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.UpdateProduto
{
    public class UpdateProdutoHandler : IRequestHandler<UpdateProdutoRequest, UpdateProdutoResponse>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public UpdateProdutoHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        public async Task<UpdateProdutoResponse> Handle(UpdateProdutoRequest request, CancellationToken cancellationToken)
        {
            var empresaId = _empresaContext.EmpresaId;
            var produto = await _context.Produtos
                .Where(p => p.EmpresaId == empresaId)
                .Include(p => p.Estoque)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (produto == null)
            {
                throw new Exception($"Produto com ID {request.Id} não encontrado");
            }

            produto.AtualizarDados(
                request.SKU,
                request.Nome,
                request.CategoriaId,
                request.GeneroId,
                request.PrecoCusto,
                request.PrecoVenda,
                request.SubcategoriaId,
                request.TamanhoId,
                request.CorId,
                request.MarcaId,
                request.FornecedorId,
                request.CodigoBarras,
                request.Foto1,
                request.Foto2,
                request.Foto3,
                request.Foto4,
                request.Foto5,
                request.DescricaoCurta,
                request.Peso
            );

            if (produto.Estoque != null)
            {
                produto.Estoque.DefinirEstoqueMinimo(request.EstoqueMinimo);
                produto.Estoque.LocalizacaoEstoque = request.LocalizacaoEstoque;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateProdutoResponse { Id = produto.Id };
        }
    }
}
