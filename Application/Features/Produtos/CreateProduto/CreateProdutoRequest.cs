using MediatR;

namespace CRMVEXA.Application.Features.Produtos.CreateProduto
{
    public class CreateProdutoRequest : IRequest<int>
    {
        public string SKU { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int? SubcategoriaId { get; set; }
        public int? TamanhoId { get; set; }
        public int? CorId { get; set; }
        public int GeneroId { get; set; }
        public int? MarcaId { get; set; }
        public int? FornecedorId { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public string? CodigoBarras { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public string? LocalizacaoEstoque { get; set; }
        public string? Foto1 { get; set; }
        public string? Foto2 { get; set; }
        public string? Foto3 { get; set; }
        public string? Foto4 { get; set; }
        public string? Foto5 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }
    }
}
