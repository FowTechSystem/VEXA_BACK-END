using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly AppDbContext _context;

        public VendaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Venda?> ObterPorIdAsync(int id) => await _context.Vendas.FindAsync(id);
        public async Task<IEnumerable<Venda>> ObterTodosAsync() => await _context.Vendas.ToListAsync();
        public async Task<Venda> AdicionarAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
            return venda;
        }
        public async Task AtualizarAsync(Venda venda)
        {
            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();
        }
        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Vendas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
