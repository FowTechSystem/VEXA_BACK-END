using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObterPorIdAsync(int id);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task<Cliente> AdicionarAsync(Cliente cliente);
        Task AtualizarAsync(Cliente cliente);
        Task RemoverAsync(int id);
    }
}
