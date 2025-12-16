namespace CRMVEXA.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; }
    public string? CPF { get; set; }
    public string? RG { get; set; } // NOVO
    public string? CNPJ { get; set; } // NOVO
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }

    public Endereco? Endereco { get; set; }

    public string? Preferencias { get; set; }
    public string? Observacoes { get; set; }
    public string? FotoUrl { get; set; } // NOVO
    public DateTime DataCadastro { get; set; }

    // Construtor protegido para EF Core
    public Cliente() { }

    public Cliente(
        string nomeCompleto,
        string? cpf,
        string telefone,
        string whatsApp,
        string? email,
        DateTime? dataNascimento,
        Endereco? endereco,
        string? preferencias,
        string? observacoes,
        DateTime dataCadastro,
        string? rg = null,
        string? cnpj = null,
        string? fotoUrl = null)
    {
        NomeCompleto = nomeCompleto;
        CPF = cpf;
        RG = rg;
        CNPJ = cnpj;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        DataNascimento = dataNascimento;
        Endereco = endereco;
        Preferencias = preferencias;
        Observacoes = observacoes;
        DataCadastro = dataCadastro;
        FotoUrl = fotoUrl;
    }

    public void AtualizarDados(
        string nomeCompleto,
        string telefone,
        string whatsApp,
        string? email = null,
        string? cpf = null,
        string? rg = null,
        string? cnpj = null,
        DateTime? dataNascimento = null,
        string? preferencias = null,
        string? observacoes = null,
        string? fotoUrl = null)
    {
        NomeCompleto = nomeCompleto;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        CPF = cpf;
        RG = rg;
        CNPJ = cnpj;
        DataNascimento = dataNascimento;
        Preferencias = preferencias;
        Observacoes = observacoes;
        FotoUrl = fotoUrl;
    }
}
