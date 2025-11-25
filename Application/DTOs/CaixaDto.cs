namespace CRMVEXA.Application.DTOs
{
    public class CaixaDto
    {
        public int Id { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }
        public int UsuarioId { get; set; }
        public string? Observacoes { get; set; }
        public bool Aberto { get; set; }
    }
}
