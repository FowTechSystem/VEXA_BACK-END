using MediatR;

namespace CRMVEXA.Application.Commands.Usuarios
{
    public record CriarUsuarioCommand(
        string Nome,
        string Email,
        string Login,
        string SenhaHash,
        string Cargo,
        bool PodeVender,
        bool PodeEditarEstoque,
        bool PodeEditarProduto,
        bool PodeAbrirFecharCaixa,
        bool PodeEmitirRelatorios,
        bool PodeCadastrarFornecedores
    ) : IRequest<int>;
}
