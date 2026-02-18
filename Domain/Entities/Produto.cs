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
        public string? CodigoBarras { get; set; }
        public string? Foto1 { get; set; }
        public string? Foto2 { get; set; }
        public string? Foto3 { get; set; }
        public string? Foto4 { get; set; }
        public string? Foto5 { get; set; }
        public string? DescricaoCurta { get; set; }
        public decimal? Peso { get; set; }

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
            string? foto1 = null,
            string? foto2 = null,
            string? foto3 = null,
            string? foto4 = null,
            string? foto5 = null,
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
            Foto1 = foto1;
            Foto2 = foto2;
            Foto3 = foto3;
            Foto4 = foto4;
            Foto5 = foto5;
            DescricaoCurta = descricaoCurta;
            Peso = peso;
        }
    }
}
