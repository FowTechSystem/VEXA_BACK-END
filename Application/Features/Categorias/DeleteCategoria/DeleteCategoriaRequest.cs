using MediatR;

namespace CRMVEXA.Application.Features.Categorias.DeleteCategoria;

public class DeleteCategoriaRequest : IRequest<DeleteCategoriaResponse>
{
    public int Id { get; set; }
}

public class DeleteCategoriaResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
