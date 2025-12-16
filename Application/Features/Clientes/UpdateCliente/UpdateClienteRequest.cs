using MediatR;

namespace CRMVEXA.Application.Features.Clientes.UpdateCliente
{
    public class UpdateClienteRequest : IRequest<UpdateClienteResponse>
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string WhatsApp { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Cpf { get; set; }
        public string? Rg { get; set; }
        public string? Cnpj { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Preferencias { get; set; }
        public string? Observacoes { get; set; }
        public string? FotoUrl { get; set; }
    }
}
