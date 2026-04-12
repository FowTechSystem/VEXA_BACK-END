using System;
using CRMVEXA.Application.Features.Produtos;
using CRMVEXA.Application.Features.Produtos.UpdateProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
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
                request.DescricaoCurta,
                request.Peso);

            var (precoPromo, emPromo) = ProdutoPromocaoHelper.NormalizarPromocao(request.PrecoPromocional);
            produto.PrecoPromocional = precoPromo;
            produto.EmPromocao = emPromo;
            produto.Observacoes = ProdutoPromocaoHelper.NormalizarObservacoes(request.Observacoes);

            if (request.Cores != null)
                produto.Cores = ProdutoListaStringHelper.Normalizar(request.Cores);
            if (request.Tamanhos != null)
                produto.Tamanhos = ProdutoListaStringHelper.Normalizar(request.Tamanhos);
            if (request.TipoTamanho != null)
                produto.TipoTamanho = string.IsNullOrWhiteSpace(request.TipoTamanho) ? null : request.TipoTamanho.Trim();
            if (request.Material != null)
                produto.Material = string.IsNullOrWhiteSpace(request.Material) ? null : request.Material.Trim();

            ProdutoFotoSlotHelper.AplicarAtualizacaoSlot(_context, produto, nameof(Produto.Foto1), request.Foto1Url, request.Foto1Base64, request.Foto1);
            ProdutoFotoSlotHelper.AplicarAtualizacaoSlot(_context, produto, nameof(Produto.Foto2), request.Foto2Url, request.Foto2Base64, request.Foto2);
            ProdutoFotoSlotHelper.AplicarAtualizacaoSlot(_context, produto, nameof(Produto.Foto3), request.Foto3Url, request.Foto3Base64, request.Foto3);
            ProdutoFotoSlotHelper.AplicarAtualizacaoSlot(_context, produto, nameof(Produto.Foto4), request.Foto4Url, request.Foto4Base64, request.Foto4);
            ProdutoFotoSlotHelper.AplicarAtualizacaoSlot(_context, produto, nameof(Produto.Foto5), request.Foto5Url, request.Foto5Base64, request.Foto5);

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
