using System.Collections.Generic;

namespace CRMVEXA.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string SKU { get; set; }
        public string Nome { get; set; }
        public int CategoriaId { get; set; }
        public int? SubcategoriaId { get; set; }
        public int? TamanhoId { get; set; }
        public int? CorId { get; set; }
        public int GeneroId { get; set; }
        public int? MarcaId { get; set; }
        public int? FornecedorId { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        /// <summary>Preço em promoção (&gt; 0) ou null se não houver promoção.</summary>
        public decimal? PrecoPromocional { get; set; }
        public bool EmPromocao { get; set; }
        /// <summary>Observações internas (uso interno da loja).</summary>
        public string? Observacoes { get; set; }
        public string? CodigoBarras { get; set; }
        /// <summary>Fotos (PostgreSQL bytea), até 5 slots.</summary>
        public byte[]? Foto1 { get; set; }
        public byte[]? Foto2 { get; set; }
        public byte[]? Foto3 { get; set; }
        public byte[]? Foto4 { get; set; }
        public byte[]? Foto5 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }

        /// <summary>Nomes de cor (português), como enviado pelo front — persistido em JSON.</summary>
        public List<string> Cores { get; set; } = new();
        /// <summary>Códigos de tamanho (ex.: P, M, 38), como enviado pelo front.</summary>
        public List<string> Tamanhos { get; set; } = new();
        /// <summary>roupas | calcados | acessorios</summary>
        public string? TipoTamanho { get; set; }
        public string? Material { get; set; }

        // Navegação
        public Categoria Categoria { get; set; } = null!;
        public Subcategoria? Subcategoria { get; set; }
        public Tamanho? Tamanho { get; set; }
        public Cor? Cor { get; set; }
        public GeneroProduto Genero { get; set; } = null!;
        public Marca? Marca { get; set; }
        public Fornecedor? Fornecedor { get; set; }
        public Estoque Estoque { get; set; } = null!;
        public Empresa Empresa { get; set; } = null!;

        private Produto() { }

        public Produto(string sku, string nome, int categoriaId, int generoId, decimal precoCusto, decimal precoVenda)
        {
            SKU = sku;
            Nome = nome;
            CategoriaId = categoriaId;
            GeneroId = generoId;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
        }

        public void AtualizarDados(
            string sku,
            string nome,
            int categoriaId,
            int generoId,
            decimal precoCusto,
            decimal precoVenda,
            int? subcategoriaId = null,
            int? tamanhoId = null,
            int? corId = null,
            int? marcaId = null,
            int? fornecedorId = null,
            string? codigoBarras = null,
            string? descricaoCurta = null,
            decimal? peso = null)
        {
            SKU = sku;
            Nome = nome;
            CategoriaId = categoriaId;
            GeneroId = generoId;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
            SubcategoriaId = subcategoriaId;
            TamanhoId = tamanhoId;
            CorId = corId;
            MarcaId = marcaId;
            FornecedorId = fornecedorId;
            CodigoBarras = codigoBarras;
            DescricaoCurta = descricaoCurta;
            Peso = peso;
        }
    }
}
