namespace CRMVEXA.Domain.Entities
{
    public class Caixa
    {
        public int Id { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DataFechamento { get; private set; }
        public decimal SaldoInicial { get; private set; }
        public decimal SaldoFinal { get; private set; }
        public int UsuarioId { get; private set; }
        public string? Observacoes { get; private set; }
        public bool Aberto { get; private set; }

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
