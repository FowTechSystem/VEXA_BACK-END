namespace CRMVEXA.Domain.Entities
{
    public class ItemVenda
    {
        public int Id { get; private set; }
        public int VendaId { get; private set; }
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public decimal Subtotal => Quantidade * PrecoUnitario;

        private ItemVenda() { }

        public ItemVenda(int produtoId, int quantidade, decimal precoUnitario)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
    }
}
