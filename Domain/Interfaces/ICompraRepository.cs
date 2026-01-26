using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface ICompraRepository
    {
        Task<Compra?> ObterPorIdAsync(int id);
        Task<IEnumerable<Compra>> ObterTodosAsync();
        Task<Compra> AdicionarAsync(Compra compra);
        Task AtualizarAsync(Compra compra);
        Task RemoverAsync(int id);
    }
}
