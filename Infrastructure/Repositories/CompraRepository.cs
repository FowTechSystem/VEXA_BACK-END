using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class CompraRepository : ICompraRepository
    {
        private readonly AppDbContext _context;

        public CompraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Compra?> ObterPorIdAsync(int id) 
            => await _context.Compras
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Compra>> ObterTodosAsync() 
            => await _context.Compras
                .Include(c => c.Itens)
                .ToListAsync();

        public async Task<Compra> AdicionarAsync(Compra compra)
        {
            await _context.Compras.AddAsync(compra);
            await _context.SaveChangesAsync();
            return compra;
        }

        public async Task AtualizarAsync(Compra compra)
        {
            _context.Compras.Update(compra);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Compras.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
