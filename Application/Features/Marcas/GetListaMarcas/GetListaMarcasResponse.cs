namespace CRMVEXA.Application.Features.Marcas.GetListaMarcas;

public class GetListaMarcasResponse
{
    public List<MarcaItemResponse> Marcas { get; set; } = new();
}

public class MarcaItemResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
}
