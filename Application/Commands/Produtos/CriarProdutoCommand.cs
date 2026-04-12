using MediatR;

namespace CRMVEXA.Application.Commands.Produtos
{
    public record CriarProdutoCommand(
        string SKU,
        string Nome,
        string Categoria,
        string? Subcategoria,
        string? Tamanho,
        string? Cor,
        string Genero,
        string? Marca,
        int? FornecedorId,
        decimal PrecoCusto,
        decimal PrecoVenda,
        string? CodigoBarras,
        int QuantidadeEstoque,
        int EstoqueMinimo,
        string? LocalizacaoEstoque,
        string? DescricaoCurta,
        decimal? Peso
    ) : IRequest<int>;
}
