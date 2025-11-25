using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        private readonly AppDbContext _context;

        public FornecedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Fornecedor?> ObterPorIdAsync(int id) => await _context.Fornecedores.FindAsync(id);
        public async Task<IEnumerable<Fornecedor>> ObterTodosAsync() => await _context.Fornecedores.ToListAsync();
        public async Task<Fornecedor> AdicionarAsync(Fornecedor fornecedor)
        {
            await _context.Fornecedores.AddAsync(fornecedor);
            await _context.SaveChangesAsync();
            return fornecedor;
        }
        public async Task AtualizarAsync(Fornecedor fornecedor)
        {
            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();
        }
        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Fornecedores.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
