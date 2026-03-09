namespace CRMVEXA.Application.Features.Catalogo.GetCatalogoPublico;

public class GetCatalogoPublicoResponse
{
    public int Id { get; set; }
    public string? EmpresaNome { get; set; }
    public string CriadorWhatsApp { get; set; } = string.Empty;
    public List<GetCatalogoPublicoItemResponse> Produtos { get; set; } = new();
}

public class GetCatalogoPublicoItemResponse
{
    public int ProdutoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; } = 1;
}
