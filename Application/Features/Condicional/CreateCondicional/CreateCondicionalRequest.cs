using System.Text.Json.Serialization;
using MediatR;

namespace CRMVEXA.Application.Features.Condicional.CreateCondicional;

public class CreateCondicionalRequest : IRequest<CreateCondicionalResponse>
{
    [JsonPropertyName("clienteId")]
    public int ClienteId { get; set; }

    [JsonPropertyName("itens")]
    public List<CreateCondicionalItemRequest> Itens { get; set; } = new();

    [JsonPropertyName("valorTotal")]
    public decimal ValorTotal { get; set; }
}

public class CreateCondicionalItemRequest
{
    [JsonPropertyName("produtoId")]
    public int ProdutoId { get; set; }

    [JsonPropertyName("quantidade")]
    public int Quantidade { get; set; }

    [JsonPropertyName("precoUnitario")]
    public decimal PrecoUnitario { get; set; }
}
