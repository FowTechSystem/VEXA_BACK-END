namespace CRMVEXA.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string? Subcategoria { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public string Genero { get; set; }
        public string? Marca { get; set; }
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

        private Produto() { }

        public Produto(string sku, string nome, string categoria, string genero, decimal precoCusto, decimal precoVenda)
        {
            SKU = sku;
            Nome = nome;
            Categoria = categoria;
            Genero = genero;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
            QuantidadeEstoque = 0;
            EstoqueMinimo = 1;
        }

        public void AtualizarEstoque(int quantidade) => QuantidadeEstoque += quantidade;
        public void DefinirEstoqueMinimo(int minimo) => EstoqueMinimo = minimo;

        public void AtualizarDados(
            string sku,
            string nome,
            string categoria,
            string genero,
            decimal precoCusto,
            decimal precoVenda,
            string? subcategoria = null,
            string? tamanho = null,
            string? cor = null,
            string? marca = null,
            int? fornecedorId = null,
            string? codigoBarras = null,
            int estoqueMinimo = 1,
            string? localizacaoEstoque = null,
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
            Categoria = categoria;
            Genero = genero;
            PrecoCusto = precoCusto;
            PrecoVenda = precoVenda;
            Subcategoria = subcategoria;
            Tamanho = tamanho;
            Cor = cor;
            Marca = marca;
            FornecedorId = fornecedorId;
            CodigoBarras = codigoBarras;
            EstoqueMinimo = estoqueMinimo;
            LocalizacaoEstoque = localizacaoEstoque;
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
