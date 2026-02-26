namespace CRMVEXA.Application.Features.Categorias.GetCategoria;

public class GetCategoriaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
