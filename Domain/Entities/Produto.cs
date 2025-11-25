namespace CRMVEXA.Domain.Entities
{
    public class Produto
    {
        public int Id { get; private set; }
        public string SKU { get; private set; }
        public string Nome { get; private set; }
        public string Categoria { get; private set; }
        public string? Subcategoria { get; private set; }
        public string? Tamanho { get; private set; }
        public string? Cor { get; private set; }
        public string Genero { get; private set; }
        public string? Marca { get; private set; }
        public int? FornecedorId { get; private set; }
        public decimal PrecoCusto { get; private set; }
        public decimal PrecoVenda { get; private set; }
        public string? CodigoBarras { get; private set; }
        public int QuantidadeEstoque { get; private set; }
        public int EstoqueMinimo { get; private set; }
        public string? LocalizacaoEstoque { get; private set; }
        public string? Foto1 { get; private set; }
        public string? Foto2 { get; private set; }
        public string? Foto3 { get; private set; }
        public string? Foto4 { get; private set; }
        public string? Foto5 { get; private set; }
        public string? DescricaoCurta { get; private set; }
        public decimal? Peso { get; private set; }

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
    }
}
