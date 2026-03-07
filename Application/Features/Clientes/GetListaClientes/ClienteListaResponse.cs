namespace CRMVEXA.Application.Features.Clientes.GetListaClientes;

public class ClienteListaResponse
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? CPF { get; set; }
    public string? RG { get; set; }
    public string? CNPJ { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Preferencias { get; set; }
    public string? Observacoes { get; set; }
    public string? FotoUrl { get; set; }
    public DateTime DataCadastro { get; set; }
    public List<EnderecoClienteResponse> Enderecos { get; set; } = new();
}

public class EnderecoClienteResponse
{
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Cep { get; set; }
}
