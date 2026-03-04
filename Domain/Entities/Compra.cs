using System.Linq;

namespace CRMVEXA.Domain.Entities
{
    public class Compra
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string CodigoCompra { get; set; }
        public int FornecedorId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime? DataEntregaPrevista { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = "pendente";
        public Empresa Empresa { get; set; } = null!;

        private List<ItemCompra> _itens = new();
        public IReadOnlyCollection<ItemCompra> Itens => _itens.AsReadOnly();

        private Compra() { }

        public Compra(int empresaId, string codigoCompra, int fornecedorId, int usuarioId, string? observacoes = null)
        {
            EmpresaId = empresaId;
            CodigoCompra = codigoCompra;
            FornecedorId = fornecedorId;
            UsuarioId = usuarioId;
            DataHora = DateTime.UtcNow;
            Observacoes = observacoes;
        }

        public void DefinirStatusEDatas(string status, DateTime dataPedido, DateTime? dataEntregaPrevista, decimal? valorTotal = null)
        {
            Status = status;
            DataHora = dataPedido;
            DataEntregaPrevista = dataEntregaPrevista;
            if (valorTotal.HasValue) ValorTotal = valorTotal.Value;
        }

        public void AdicionarItem(ItemCompra item)
        {
            _itens.Add(item);
            CalcularTotal();
        }

        private void CalcularTotal()
        {
            if (_itens.Count > 0)
                ValorTotal = _itens.Sum(i => i.Subtotal);
        }

        public void Cancelar() => Status = "cancelada";
        public void MarcarRecebida() => Status = "recebida";
    }
}
