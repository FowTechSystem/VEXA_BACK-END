using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class CaixaRepository : ICaixaRepository
    {
        private readonly AppDbContext _context;

        public CaixaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Caixa?> ObterPorIdAsync(int id) => await _context.Caixas.FindAsync(id);
        public async Task<IEnumerable<Caixa>> ObterTodosAsync() => await _context.Caixas.ToListAsync();
        public async Task<Caixa> AdicionarAsync(Caixa caixa)
        {
            await _context.Caixas.AddAsync(caixa);
            await _context.SaveChangesAsync();
            return caixa;
        }
        public async Task AtualizarAsync(Caixa caixa)
        {
            _context.Caixas.Update(caixa);
            await _context.SaveChangesAsync();
        }
        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Caixas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
