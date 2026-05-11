using System.Collections.Generic;
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
        public decimal? PrecoPromocional { get; set; }
        public bool EmPromocao { get; set; }
        public string? Observacoes { get; set; }
        public string? CodigoBarras { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int EstoqueMinimo { get; set; }
        public string? LocalizacaoEstoque { get; set; }
        /// <summary>Data URL ou base64 puro (alias legado do front).</summary>
        public string? Foto1 { get; set; }
        public string? Foto2 { get; set; }
        public string? Foto3 { get; set; }
        public string? Foto4 { get; set; }
        public string? Foto5 { get; set; }
        /// <summary>Mesmo payload que <see cref="Foto1"/> — padrão criar usuário/cliente.</summary>
        public string? Foto1Base64 { get; set; }
        public string? Foto2Base64 { get; set; }
        public string? Foto3Base64 { get; set; }
        public string? Foto4Base64 { get; set; }
        public string? Foto5Base64 { get; set; }
        /// <summary>
        /// Galeria adicional (além da principal). Ordem preservada; cada item pode ser URL http(s) ou data URL/base64.
        /// Mapeia para os slots Foto2..Foto5.
        /// </summary>
        public List<string>? FotosAdicionaisUrls { get; set; }
        /// <summary>
        /// Galeria adicional em base64 puro (sem prefixo data URI). Ordem preservada.
        /// Mapeia para os slots Foto2..Foto5.
        /// </summary>
        public List<string>? FotosAdicionaisBase64 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }

        /// <summary>Nomes de cor em português (catálogo do front), não IDs.</summary>
        public List<string>? Cores { get; set; }
        public List<string>? Tamanhos { get; set; }
        /// <summary>roupas | calcados | acessorios</summary>
        public string? TipoTamanho { get; set; }
        public string? Material { get; set; }
    }
}
