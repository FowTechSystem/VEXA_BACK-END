using CRMVEXA.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissoesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissoesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todas as permissões disponíveis (para o front montar checkboxes e enviar PermissaoIds ao salvar usuário).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissaoDto>>> Listar()
        {
            var list = await _context.Permissoes
                .AsNoTracking()
                .OrderBy(p => p.Nome)
                .Select(p => new PermissaoDto { Id = p.Id, Nome = p.Nome, Codigo = p.Codigo })
                .ToListAsync();
            return Ok(list);
        }
    }

    public class PermissaoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
    }
}
