using Microsoft.AspNetCore.Mvc;
using MediatR;


namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<ActionResult<int>> Criar([FromBody] CriarVendaCommand command)
        //{
        //    var id = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        //}

        //[HttpPost("cancelar")]
        //public async Task<ActionResult> Cancelar([FromBody] CancelarVendaCommand command)
        //{
        //    await _mediator.Send(command);
        //    return NoContent();
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<VendaDto>> ObterPorId(int id)
        //{
        //    // Implemente a query: ObterVendaPorIdQuery
        //    return Ok(); // placeholder
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<VendaDto>>> Listar()
        //{
        //    // Implemente a query: ListarVendasQuery
        //    return Ok(); // placeholder
        //}
    }
}
