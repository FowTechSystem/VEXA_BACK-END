namespace CRMVEXA.Application.Features.Condicional.GetCondicional;

public class GetCondicionalResponse
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public int ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<GetCondicionalItemResponse> Itens { get; set; } = new();
}

public class GetCondicionalItemResponse
{
    public int ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
