namespace CRMVEXA.Application.Features.Condicional.DevolverCondicional;

public class DevolverCondicionalResponse
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public int ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "devolvido";
    public List<DevolverCondicionalItemResponse> Itens { get; set; } = new();
}

public class DevolverCondicionalItemResponse
{
    public int ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
