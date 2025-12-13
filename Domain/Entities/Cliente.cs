namespace CRMVEXA.Domain.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public string NomeCompleto { get; set; }
    public string? CPF { get; set; }
    public string Telefone { get; set; }
    public string WhatsApp { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
    public Endereco Endereco { get; set; }
    public string? Preferencias { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; }

    // Construtor protegido para EF Core
    public Cliente() { }

    public Cliente(string nomeCompleto, string? cPF, string telefone, string whatsApp, string? email,
        DateTime? dataNascimento, Endereco endereco, string? preferencias, string? observacoes, DateTime dataCadastro)
    {
        NomeCompleto = nomeCompleto;
        CPF = cPF;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        DataNascimento = dataNascimento;
        Endereco = endereco;
        Preferencias = preferencias;
        Observacoes = observacoes;
        DataCadastro = dataCadastro;
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
