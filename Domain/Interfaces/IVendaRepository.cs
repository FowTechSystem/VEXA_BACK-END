using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface IVendaRepository
    {
        Task<Venda?> ObterPorIdAsync(int id);
        Task<IEnumerable<Venda>> ObterTodosAsync();
        Task<Venda> AdicionarAsync(Venda venda);
        Task AtualizarAsync(Venda venda);
        Task RemoverAsync(int id);
    }
}
