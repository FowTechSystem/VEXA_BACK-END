using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CriarClienteCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> ObterPorId(int id)
        {
            // Implemente a Query (ObterClientePorIdQuery) na Application/Queries se quiser GET completo
            return Ok(); // Placeholder
        }
    }
}
