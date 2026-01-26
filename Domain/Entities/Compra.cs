using System.Linq;

namespace CRMVEXA.Domain.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public string CodigoCompra { get; set; }
        public int FornecedorId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = "Concluída";

        private List<ItemCompra> _itens = new();
        public IReadOnlyCollection<ItemCompra> Itens => _itens.AsReadOnly();

        private Compra() { }

        public Compra(string codigoCompra, int fornecedorId, int usuarioId, string? observacoes = null)
        {
            CodigoCompra = codigoCompra;
            FornecedorId = fornecedorId;
            UsuarioId = usuarioId;
            DataHora = DateTime.UtcNow;
            Observacoes = observacoes;
        }

        public void AdicionarItem(ItemCompra item)
        {
            _itens.Add(item);
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            ValorTotal = _itens.Sum(i => i.Subtotal);
        }

        public void Cancelar() => Status = "Cancelada";
    }
}
