using System.Collections.Generic;

namespace CRMVEXA.Application.Features.Produtos.GetListaProdutos
{
    public class GetListaProdutosResponse
    {
        public List<ProdutoResponse> Produtos { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
        public int TamanhoPagina { get; set; }
    }

    public class ProdutoResponse
    {
        /// <summary>normal (estoque) | bazar (peça consignada disponível)</summary>
        public string Tipo { get; set; } = "normal";
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        /// <summary>Consignante, quando <see cref="Tipo"/> é bazar.</summary>
        public string? ConsignanteNome { get; set; }
        /// <summary>disponivel | vendida | devolvida — apenas bazar.</summary>
        public string? StatusBazar { get; set; }
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
        public decimal? PrecoPromocional { get; set; }
        public bool EmPromocao { get; set; }
        public string? Observacoes { get; set; }
        public string? CodigoBarras { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public int EstoqueMaximo { get; set; }
        public string? CategoriaNome { get; set; }
        public string? LocalizacaoEstoque { get; set; }
        public string? Foto1 { get; set; }
        public string? Foto2 { get; set; }
        public string? Foto3 { get; set; }
        public string? Foto4 { get; set; }
        public string? Foto5 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }
        public List<string> Cores { get; set; } = new();
        public List<string> Tamanhos { get; set; } = new();
        public string? TipoTamanho { get; set; }
        public string? Material { get; set; }
    }

    public class CategoriaRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    public class MarcaRefResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
