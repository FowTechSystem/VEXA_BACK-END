namespace CRMVEXA.Application.Features.Catalogo.GetListaCatalogos;

public class GetListaCatalogosResponse
{
    public List<CatalogoListaItem> Items { get; set; } = new();
    public int Total { get; set; }
    public int Pagina { get; set; }
}

public class CatalogoListaItem
{
    public int Id { get; set; }
    public string? EmpresaNome { get; set; }
    public string LinkPublico { get; set; } = string.Empty;
    public int QuantidadeProdutos { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Ativo { get; set; }
}
