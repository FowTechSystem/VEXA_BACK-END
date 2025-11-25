using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface ICaixaRepository
    {
        Task<Caixa?> ObterPorIdAsync(int id);
        Task<IEnumerable<Caixa>> ObterTodosAsync();
        Task<Caixa> AdicionarAsync(Caixa caixa);
        Task AtualizarAsync(Caixa caixa);
        Task RemoverAsync(int id);
    }
}
