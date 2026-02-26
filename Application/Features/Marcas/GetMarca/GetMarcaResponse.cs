namespace CRMVEXA.Application.Features.Marcas.GetMarca;

public class GetMarcaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
