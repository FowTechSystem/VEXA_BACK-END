namespace CRMVEXA.Domain.Entities
{
    public class Fornecedor
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string? RazaoSocial { get; private set; }
        public string? CNPJ { get; private set; }
        public string? NomeRepresentante { get; private set; }
        public string Telefone { get; private set; }
        public string? WhatsApp { get; private set; }
        public string? Email { get; private set; }
        public string? Endereco { get; private set; }
        public string? CategoriaProdutos { get; private set; }
        public string? CondicaoPagamento { get; private set; }
        public string? Observacoes { get; private set; }
        public DateTime DataCadastro { get; private set; }

        private Fornecedor() { }

        public Fornecedor(string nome, string telefone)
        {
            Nome = nome;
            Telefone = telefone;
            DataCadastro = DateTime.UtcNow;
        }
    }
}
