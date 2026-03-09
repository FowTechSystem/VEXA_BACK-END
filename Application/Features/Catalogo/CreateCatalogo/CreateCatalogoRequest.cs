using System.Text.Json.Serialization;
using MediatR;

namespace CRMVEXA.Application.Features.Catalogo.CreateCatalogo;

public class CreateCatalogoRequest : IRequest<CreateCatalogoResponse>
{
    [JsonPropertyName("produtos")]
    public List<CreateCatalogoItemRequest> Produtos { get; set; } = new();

    [JsonPropertyName("criadorNome")]
    public string? CriadorNome { get; set; }

    [JsonPropertyName("criadorWhatsApp")]
    public string CriadorWhatsApp { get; set; } = string.Empty;

    [JsonPropertyName("empresaNome")]
    public string? EmpresaNome { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
}

public class CreateCatalogoItemRequest
{
    [JsonPropertyName("produtoId")]
    public int ProdutoId { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("preco")]
    public decimal Preco { get; set; }

    [JsonPropertyName("quantidade")]
    public int Quantidade { get; set; } = 1;
}
