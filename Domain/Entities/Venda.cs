using System.Linq;

namespace CRMVEXA.Domain.Entities
{
    public class Venda
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string CodigoVenda { get; set; }
        public int? ClienteId { get; set; }
        public decimal? Desconto { get; set; }
        public string FormaPagamento { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "Concluída";
        public Empresa Empresa { get; set; } = null!;

        private List<ItemVenda> _itens = new();
        public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

        private Venda() { }

        public Venda(int empresaId, string codigoVenda, int usuarioId, string formaPagamento)
        {
            EmpresaId = empresaId;
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
            ValorTotal = _itens.Sum(i => i.Subtotal) - (Desconto ?? 0);
        }

        public void Cancelar() => Status = "Cancelada";

        public void AtualizarCliente(int? clienteId)
        {
            ClienteId = clienteId;
        }

        public void RemoverItem(ItemVenda item)
        {
            _itens.Remove(item);
            CalcularTotal();
        }
    }
}
