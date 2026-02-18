using System;
using CRMVEXA.Application.Features.Produtos.CreateProduto;
using CRMVEXA.Application.Services;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Infrastructure.Data;
using MediatR;

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

        public async Task<int> Handle(CreateProdutoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var empresaId = _empresaContext.EmpresaId;
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

                var estoque = new Estoque(empresaId, 0, request.QuantidadeEstoque, request.EstoqueMinimo, request.LocalizacaoEstoque);
                produto.Estoque = estoque;

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
