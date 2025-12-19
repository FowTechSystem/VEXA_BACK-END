namespace CRMVEXA.Application.Features.Fornecedor.UpdateFornecedor
{
    public class UpdateFornecedorResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? Fornecedorid { get; set; }
    }
}
