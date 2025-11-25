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

        private Cliente() { }

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

        public void Atualizar(string nomeCompleto, string telefone, string whatsApp, string? email)
        {
            NomeCompleto = nomeCompleto;
            Telefone = telefone;
            WhatsApp = whatsApp;
            Email = email;
        }
    }
}
