using System.Text.Json.Serialization;
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
        /// <summary>Campos flat do endereço (podem vir no root).</summary>
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Cep { get; set; }
        /// <summary>Endereço aninhado. Se enviado, terá prioridade sobre Rua, Numero, etc.</summary>
        public EnderecoUpdateRequest? Endereco { get; set; }
        /// <summary>Array de endereços (formato GET). Usa o primeiro item se enviado.</summary>
        public List<EnderecoUpdateRequest>? Enderecos { get; set; }
        public string? Preferencias { get; set; }
        public string? Observacoes { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class EnderecoUpdateRequest
    {
        public string? Cep { get; set; }
        public string? Rua { get; set; }
        [JsonPropertyName("endereco")]
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Complemento { get; set; }
    }
}
