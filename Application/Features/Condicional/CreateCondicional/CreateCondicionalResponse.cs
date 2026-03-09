namespace CRMVEXA.Application.Features.Condicional.CreateCondicional;

public class CreateCondicionalResponse
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public int ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "pendente";
    public List<CreateCondicionalItemResponse> Itens { get; set; } = new();
}

public class CreateCondicionalItemResponse
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string? ProdutoNome { get; set; }
}
