using MediatR;

namespace CRMVEXA.Application.Commands.Usuarios
{
    public record AtualizarUsuarioCommand(
        int Id,
        string Nome,
        string Email,
        string Login,
        string Cargo,
        bool PodeVender,
        bool PodeEditarEstoque,
        bool PodeEditarProduto,
        bool PodeAbrirFecharCaixa,
        bool PodeEmitirRelatorios,
        bool PodeCadastrarFornecedores
    ) : IRequest<bool>;
}
