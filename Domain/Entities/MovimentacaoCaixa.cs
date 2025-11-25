namespace CRMVEXA.Domain.Entities
{
    public class MovimentacaoCaixa
    {
        public int Id { get; private set; }
        public int CaixaId { get; private set; }
        public decimal Valor { get; private set; }
        public string Tipo { get; private set; } // Entrada ou Saída
        public string FormaPagamento { get; private set; }
        public string? Descricao { get; private set; }
        public DateTime DataHora { get; private set; }

        private MovimentacaoCaixa() { }

        public MovimentacaoCaixa(int caixaId, string tipo, decimal valor, string formaPagamento, string? descricao = null)
        {
            CaixaId = caixaId;
            Tipo = tipo;
            Valor = valor;
            FormaPagamento = formaPagamento;
            Descricao = descricao;
            DataHora = DateTime.UtcNow;
        }
    }
}
