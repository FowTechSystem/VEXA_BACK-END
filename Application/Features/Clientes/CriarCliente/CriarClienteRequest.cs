using System.Text.Json.Serialization;
using MediatR;

namespace CRMVEXA.Application.Features.Clientes.CriarCliente;

public class CriarClienteRequest : IRequest<CriarClienteResponse>
{
    public string nomeCompleto { get; set; } = string.Empty;
    public string cPF { get; set; } = string.Empty;
    public string? telefone { get; set; }
    public string? whatsApp { get; set; }
    public string? email { get; set; }
    public DateTime? dataNascimento { get; set; }
    public EnderecoClienteRequest? endereco { get; set; }
    public string? preferencias { get; set; }
    public string? observacoes { get; set; }
    public DateTime dataCadastro { get; set; }
    public string? rg { get; set; }
    public string? cnpj { get; set; }
    public string? FotoBase64 { get; set; }

    /// <summary>Endereço no root (formato flat esperado pelo front). Usado quando endereco não vier preenchido.</summary>
    public string? rua { get; set; }
    public string? numero { get; set; }
    public string? complemento { get; set; }
    public string? bairro { get; set; }
    public string? cidade { get; set; }
    public string? cep { get; set; }
}

/// <summary>Endereço do cliente. Aceita "rua" ou "endereco" para o nome da rua.</summary>
public class EnderecoClienteRequest
{
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    /// <summary>Alias para Rua (nome da rua / logradouro).</summary>
    [JsonPropertyName("endereco")]
    public string? Endereco { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Complemento { get; set; }
}
