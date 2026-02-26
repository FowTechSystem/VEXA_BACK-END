using CRMVEXA.Application.Features.Produtos.Common;

namespace CRMVEXA.Application.Features.Produtos.GetProduto
{
    public class GetProdutoResponse
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public CategoriaRefResponse? Categoria { get; set; }
        public string? Subcategoria { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public string Genero { get; set; } = string.Empty;
        public int? MarcaId { get; set; }
        public MarcaRefResponse? Marca { get; set; }
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
