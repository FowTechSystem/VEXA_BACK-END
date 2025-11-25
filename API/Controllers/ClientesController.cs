using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Commands.Clientes;
using CRMVEXA.Application.DTOs;

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
        public async Task<ActionResult<int>> CreateCliente([FromBody] CriarClienteCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> ObterPorId(int id)
        {
            // Implemente a query: ObterClientePorIdQuery
            // Exemplo:
            // var cliente = await _mediator.Send(new ObterClientePorIdQuery(id));
            // return Ok(cliente);
            return Ok(); // placeholder
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> Listar()
        {
            // Implemente a query: ListarClientesQuery
            // Exemplo:
            // var clientes = await _mediator.Send(new ListarClientesQuery());
            // return Ok(clientes);
            return Ok(); // placeholder
        }
    }
}
