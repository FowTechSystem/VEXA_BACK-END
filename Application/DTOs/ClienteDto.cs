namespace CRMVEXA.Application.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public string WhatsApp { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public EnderecoDto Endereco { get; set; } = new();
        public string? Preferencias { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class EnderecoDto
    {
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
    }
}
