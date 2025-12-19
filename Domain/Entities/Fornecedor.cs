namespace CRMVEXA.Domain.Entities;

public class Fornecedor
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? RazaoSocial { get; set; }
    public string? CNPJ { get; set; }
    public string? NomeRepresentante { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public Endereco Endereco { get; set; }
    public string? CategoriaProdutos { get; set; }
    public string? CondicaoPagamento { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; }

    public ICollection<Endereco> Enderecos { get; set; }

    public Fornecedor() { }

    public Fornecedor(string nome, string? razaoSocial, string? cNPJ, string? nomeRepresentante,
        string? telefone, string? whatsApp, string? email, Endereco endereco, string? categoriaProdutos,
        string? condicaoPagamento, string? observacoes, DateTime dataCadastro)
    {
        Nome = nome;
        RazaoSocial = razaoSocial;
        CNPJ = cNPJ;
        NomeRepresentante = nomeRepresentante;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        Endereco = endereco;
        CategoriaProdutos = categoriaProdutos;
        CondicaoPagamento = condicaoPagamento;
        Observacoes = observacoes;
        DataCadastro = dataCadastro;
    }

    public void AtualizarDados(string nome, string? razaoSocial, string? cnpj, string? nomeRepresentante,
        string? telefone, string? whatsApp, string? email, Endereco endereco, string? categoriaProdutos,
        string? condicaoPagamento, string? observacoes, DateTime dataCadastro)
    {
        Nome = nome;
        RazaoSocial = razaoSocial;
        CNPJ = cnpj;
        NomeRepresentante = nomeRepresentante;
        Telefone = telefone;
        WhatsApp = whatsApp;
        Email = email;
        Endereco = endereco;
        CategoriaProdutos = categoriaProdutos;
        CondicaoPagamento = condicaoPagamento;
        Observacoes = observacoes;
        DataCadastro = dataCadastro;
    }

}
