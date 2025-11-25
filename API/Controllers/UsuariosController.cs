using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Commands.Usuarios;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CriarUsuarioCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> ObterPorId(int id)
        {
            // Implemente a query: ObterUsuarioPorIdQuery
            return Ok(); // placeholder
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> Listar()
        {
            // Implemente a query: ListarUsuariosQuery
            return Ok(); // placeholder
        }
    }
}
