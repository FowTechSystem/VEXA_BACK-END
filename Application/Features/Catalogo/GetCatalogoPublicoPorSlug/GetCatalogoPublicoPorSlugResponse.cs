namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublicoPorSlug;

public class GetCatalogoPublicoPorSlugResponse
{
    public int Id { get; set; }
    public string? EmpresaNome { get; set; }
    public string CriadorWhatsApp { get; set; } = string.Empty;
    public List<GetCatalogoPublicoPorSlugItemResponse> Produtos { get; set; } = new();
}

public class GetCatalogoPublicoPorSlugItemResponse
{
    public int ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; } = 1;
}
