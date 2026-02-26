using MediatR;

namespace CRMVEXA.Application.Features.Categorias.UpdateCategoria;

public class UpdateCategoriaRequest : IRequest<UpdateCategoriaResponse>
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class UpdateCategoriaResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
