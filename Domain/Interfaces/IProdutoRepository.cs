using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);
        Task<IEnumerable<Produto>> ObterTodosAsync();
        Task<Produto> AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task RemoverAsync(int id);
    }
}
