namespace CRMVEXA.Application.Features.Categorias.GetListaCategorias;

public class GetListaCategoriasResponse
{
    public List<CategoriaItemResponse> Categorias { get; set; } = new();
}

public class CategoriaItemResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
}
