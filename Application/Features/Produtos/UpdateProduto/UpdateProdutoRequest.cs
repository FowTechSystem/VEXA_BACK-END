using System.Collections.Generic;
using MediatR;

namespace CRMVEXA.Application.Features.Produtos.UpdateProduto
{
    public class UpdateProdutoRequest : IRequest<UpdateProdutoResponse>
    {
        public int Id { get; set; }
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
        public int EstoqueMinimo { get; set; }
        public string? LocalizacaoEstoque { get; set; }
        /// <summary>Por slot: null = não alterar; vazio = remover foto; data URL/base64 = gravar (após upload).</summary>
        public string? Foto1Url { get; set; }
        public string? Foto2Url { get; set; }
        public string? Foto3Url { get; set; }
        public string? Foto4Url { get; set; }
        public string? Foto5Url { get; set; }
        public string? Foto1Base64 { get; set; }
        public string? Foto2Base64 { get; set; }
        public string? Foto3Base64 { get; set; }
        public string? Foto4Base64 { get; set; }
        public string? Foto5Base64 { get; set; }
        /// <summary>Legado: mesmo significado que <see cref="Foto1Url"/> (data URL/base64).</summary>
        public string? Foto1 { get; set; }
        public string? Foto2 { get; set; }
        public string? Foto3 { get; set; }
        public string? Foto4 { get; set; }
        public string? Foto5 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }

        /// <summary>Se null, não altera. Se enviado (inclui []), substitui a lista.</summary>
        public List<string>? Cores { get; set; }
        public List<string>? Tamanhos { get; set; }
        public string? TipoTamanho { get; set; }
        public string? Material { get; set; }
    }

    public class UpdateProdutoResponse
    {
        public int Id { get; set; }
        public string Mensagem { get; set; } = "Produto atualizado com sucesso";
    }
}
