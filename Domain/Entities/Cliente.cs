namespace CRMVEXA.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int EmpresaId { get; set; }
    public string NomeCompleto { get; set; }
    public string? CPF { get; set; }
    public string? RG { get; set; }
    public string? CNPJ { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Preferencias { get; set; }
    public string? Observacoes { get; set; }
    public string? FotoBase64 { get; set; }
    public DateTime DataCadastro { get; set; }

    public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
    public Empresa Empresa { get; set; } = null!;

    public Cliente() { }

    public Cliente(
        string nomeCompleto,
        string? cpf,
        string telefone,
        string whatsApp,
        string? email,
        DateTime? dataNascimento,
        string? preferencias,
        string? observacoes,
        DateTime dataCadastro,
        string? rg = null,
        string? cnpj = null,
        string? fotoBase64 = null)
    {
        NomeCompleto = nomeCompleto;
        CPF = cpf;
        RG = rg;
        CNPJ = cnpj;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        DataNascimento = dataNascimento;
        Preferencias = preferencias;
        Observacoes = observacoes;
        DataCadastro = dataCadastro;
        FotoBase64 = fotoBase64;
        Enderecos = new List<Endereco>();
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
        string? fotoBase64 = null)
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
        FotoBase64 = fotoBase64;
    }


    public void AdicionarEndereco(Endereco endereco)
    {
        if (!Enderecos.Contains(endereco))
        {
            Enderecos.Add(endereco);
        }
    }

    public void RemoverEndereco(Endereco endereco)
    {
        Enderecos.Remove(endereco);
    }
}
