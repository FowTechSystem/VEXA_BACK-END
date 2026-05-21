namespace CRMVEXA.Domain.Entities
{
    public class ItemVenda
    {
        public int Id { get; set; }
        public int VendaId { get; set; }
        /// <summary>Produto de estoque normal. Nulo quando <see cref="TipoItem"/> é bazar.</summary>
        public int? ProdutoId { get; set; }
        /// <summary>normal | bazar</summary>
        public string TipoItem { get; set; } = "normal";
        public int? BazarProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        private ItemVenda() { }

        public ItemVenda(int produtoId, int quantidade, decimal precoUnitario)
        {
            ProdutoId = produtoId;
            TipoItem = "normal";
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }

        public ItemVenda(int bazarProdutoId, decimal precoUnitario)
        {
            BazarProdutoId = bazarProdutoId;
            TipoItem = "bazar";
            Quantidade = 1;
            PrecoUnitario = precoUnitario;
        }
    }
}
