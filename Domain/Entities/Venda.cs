namespace CRMVEXA.Domain.Entities
{
    public class Venda
    {
        public int Id { get; private set; }
        public string CodigoVenda { get; private set; }
        public int? ClienteId { get; private set; }
        public decimal Desconto { get; private set; }
        public string FormaPagamento { get; private set; }
        public int UsuarioId { get; private set; }
        public DateTime DataHora { get; private set; }
        public decimal ValorTotal { get; private set; }
        public string Status { get; private set; } = "Concluída";

        private List<ItemVenda> _itens = new();
        public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

        private Venda() { }

        public Venda(string codigoVenda, int usuarioId, string formaPagamento)
        {
            CodigoVenda = codigoVenda;
            FormaPagamento = formaPagamento;
            UsuarioId = usuarioId;
            DataHora = DateTime.UtcNow;
        }

        public void AdicionarItem(ItemVenda item)
        {
            _itens.Add(item);
            CalcularTotal();
        }

        public void AplicarDesconto(decimal valorDesconto)
        {
            Desconto = valorDesconto;
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            ValorTotal = _itens.Sum(i => i.Subtotal) - Desconto;
        }

        public void Cancelar() => Status = "Cancelada";
    }
}
