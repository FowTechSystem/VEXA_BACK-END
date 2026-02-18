namespace CRMVEXA.Application.Features.Empresas.GetListaEmpresas
{
    public class GetListaEmpresasResponse
    {
        public List<EmpresaResponse> Empresas { get; set; } = new();
    }

    public class EmpresaResponse
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? NomeFantasia { get; set; }
        public string? Cnpj { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
