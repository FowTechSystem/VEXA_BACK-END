namespace CRMVEXA.Domain.Entities
{
    public class MovimentacaoCaixa
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int CaixaId { get; set; }
        public decimal Valor { get; set; }
        public string Tipo { get; set; } // Entrada ou Saída
        public string FormaPagamento { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataHora { get; set; }

        private MovimentacaoCaixa() { }

        public MovimentacaoCaixa(int empresaId, int caixaId, string tipo, decimal valor, string formaPagamento, string? descricao = null)
        {
            EmpresaId = empresaId;
            CaixaId = caixaId;
            Tipo = tipo;
            Valor = valor;
            FormaPagamento = formaPagamento;
            Descricao = descricao;
            DataHora = DateTime.UtcNow;
        }
    }
}
