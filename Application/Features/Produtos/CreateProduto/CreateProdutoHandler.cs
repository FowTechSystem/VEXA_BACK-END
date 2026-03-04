using System;
using CRMVEXA.Application.Features.Produtos.CreateProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Application.Features.Produtos.CreateProduto
{
    public class CreateProdutoHandler : IRequestHandler<CreateProdutoRequest, int>
    {
        private readonly AppDbContext _context;
        private readonly IEmpresaContext _empresaContext;

        public CreateProdutoHandler(AppDbContext context, IEmpresaContext empresaContext)
        {
            _context = context;
            _empresaContext = empresaContext;
        }

        private static string ObterMensagemErro(Exception ex)
        {
            var e = ex;
            while (e?.InnerException != null)
                e = e.InnerException;
            return e?.Message ?? ex.Message;
        }

        public async Task<int> Handle(CreateProdutoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _empresaContext.EmpresaId;

                // Validar FKs antes de inserir
                var catExiste = await _context.Categorias.AnyAsync(c => c.EmpresaId == empresaId && c.Id == request.CategoriaId, cancellationToken);
                if (!catExiste)
                    throw new Exception($"Categoria com Id {request.CategoriaId} não encontrada. Cadastre categorias em /api/categorias primeiro.");

                var genExiste = await _context.GenerosProduto.AnyAsync(g => g.Id == request.GeneroId, cancellationToken);
                if (!genExiste)
                    throw new Exception($"Gênero com Id {request.GeneroId} não encontrado. Verifique os gêneros cadastrados.");

                if (request.MarcaId.HasValue)
                {
                    var marcaExiste = await _context.Marcas.AnyAsync(m => m.EmpresaId == empresaId && m.Id == request.MarcaId, cancellationToken);
                    if (!marcaExiste)
                        throw new Exception($"Marca com Id {request.MarcaId} não encontrada.");
                }
                var produto = new Produto(
                    request.SKU,
                    request.Nome,
                    request.CategoriaId,
                    request.GeneroId,
                    request.PrecoCusto,
                    request.PrecoVenda)
                {
                    EmpresaId = empresaId,
                    SubcategoriaId = request.SubcategoriaId,
                    TamanhoId = request.TamanhoId,
                    CorId = request.CorId,
                    MarcaId = request.MarcaId,
                    FornecedorId = request.FornecedorId,
                    CodigoBarras = request.CodigoBarras,
                    Foto1 = request.Foto1,
                    Foto2 = request.Foto2,
                    Foto3 = request.Foto3,
                    Foto4 = request.Foto4,
                    Foto5 = request.Foto5,
                    DescricaoCurta = request.DescricaoCurta,
                    Peso = request.Peso
                };

                var estoque = new CRMVEXA.Domain.Entities.Estoque(empresaId, 0, request.QuantidadeEstoque, request.EstoqueMinimo, request.LocalizacaoEstoque);
                produto.Estoque = estoque;

                _context.Produtos.Add(produto);
                await _context.SaveChangesAsync(cancellationToken);

                return produto.Id;
            }
            catch (Exception ex)
            {
                var msg = ObterMensagemErro(ex);
                throw new Exception($"Erro ao criar produto: {msg}", ex);
            }
        }
    }
}
