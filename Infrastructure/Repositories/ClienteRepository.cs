using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObterPorIdAsync(int id) => await _context.Clientes.FindAsync(id);
        public async Task<IEnumerable<Cliente>> ObterTodosAsync() => await _context.Clientes.ToListAsync();
        public async Task<Cliente> AdicionarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }
        public async Task AtualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Clientes.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
