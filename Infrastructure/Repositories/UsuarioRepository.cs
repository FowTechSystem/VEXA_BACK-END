using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.Interfaces;
using CRMVEXA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorIdAsync(int id) => await _context.Usuarios.FindAsync(id);
        public async Task<IEnumerable<Usuario>> ObterTodosAsync() => await _context.Usuarios.ToListAsync();
        public async Task<Usuario> AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task RemoverAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _context.Usuarios.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
