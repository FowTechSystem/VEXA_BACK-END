namespace CRMVEXA.Domain.Entities
{
    public class Caixa
    {
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        public int UsuarioId { get; set; }
        public string? Observacoes { get; set; }
        public bool Aberto { get; set; }

        // Construtor protegido para uso do Entity Framework
        protected Caixa() { }

        // Construtor público para criação manual
        public Caixa(int usuarioId, decimal saldoInicial)
        {
            UsuarioId = usuarioId;
            SaldoInicial = saldoInicial;
            SaldoFinal = saldoInicial;
            DataAbertura = DateTime.UtcNow;
            Aberto = true;
        }

        public void Fechar(decimal saldoFinal, string? observacoes)
        {
            SaldoFinal = saldoFinal;
            DataFechamento = DateTime.UtcNow;
            Observacoes = observacoes;
            Aberto = false;
        }
    }
}
