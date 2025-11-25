using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface IFornecedorRepository
    {
        Task<Fornecedor?> ObterPorIdAsync(int id);
        Task<IEnumerable<Fornecedor>> ObterTodosAsync();
        Task<Fornecedor> AdicionarAsync(Fornecedor fornecedor);
        Task AtualizarAsync(Fornecedor fornecedor);
        Task RemoverAsync(int id);
    }
}
