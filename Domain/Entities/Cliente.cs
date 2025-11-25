using CRMVEXA.Domain.ValueObjects;

namespace CRMVEXA.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; private set; }
        public string NomeCompleto { get; private set; }
        public string? CPF { get; private set; }
        public string Telefone { get; private set; }
        public string WhatsApp { get; private set; }
        public string? Email { get; private set; }
        public DateTime? DataNascimento { get; private set; }
        public Endereco Endereco { get; private set; }
        public string? Preferencias { get; private set; }
        public string? Observacoes { get; private set; }
        public DateTime DataCadastro { get; private set; }

        // Construtor protegido para EF Core
        protected Cliente() { }

        // Construtor público para criação manual
        public Cliente(
            string nomeCompleto,
            string telefone,
            string whatsApp,
            Endereco endereco)
        {
            NomeCompleto = nomeCompleto;
            Telefone = telefone;
            WhatsApp = whatsApp;
            Endereco = endereco;
            DataCadastro = DateTime.UtcNow;
        }

        public void AtualizarDados(
            string nomeCompleto,
            string telefone,
            string whatsApp,
            string? email = null,
            string? cpf = null,
            DateTime? dataNascimento = null,
            string? preferencias = null,
            string? observacoes = null)
        {
            NomeCompleto = nomeCompleto;
            Telefone = telefone;
            WhatsApp = whatsApp;
            Email = email;
            CPF = cpf;
            DataNascimento = dataNascimento;
            Preferencias = preferencias;
            Observacoes = observacoes;
        }
    }
}
