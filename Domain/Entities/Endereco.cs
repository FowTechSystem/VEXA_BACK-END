namespace CRMVEXA.Domain.Entities;

public class Endereco
{
    public string Rua { get; set; }
    public string Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Cep { get; set; }
    public string? Complemento { get; set; }

    // Construtor protegido para EF Core
    public Endereco() { }

    public Endereco(string rua, string numero, string bairro, string cidade, string cep, string? complemento = null)
    {
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Cep = cep;
        Complemento = complemento;
    }

    public void AtualizarEndereco(string rua, string numero, string bairro, string cidade, string cep, string? complemento = null)
    {
        Rua = rua;
        Numero = numero;
        Bairro = bairro;
        Cidade = cidade;
        Cep = cep;
        Complemento = complemento;
    }
}
