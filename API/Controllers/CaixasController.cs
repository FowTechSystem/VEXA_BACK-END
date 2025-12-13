using Microsoft.AspNetCore.Mvc;
using MediatR;


namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaixasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaixasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost("abrir")]
        //public async Task<ActionResult<int>> Abrir([FromBody] AbrirCaixaCommand command)
        //{
        //    var id = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        //}

        //[HttpPost("fechar")]
        //public async Task<ActionResult> Fechar([FromBody] FecharCaixaCommand command)
        //{
        //    await _mediator.Send(command);
        //    return NoContent();
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<CaixaDto>> ObterPorId(int id)
        //{
        //    // Implemente a query: ObterCaixaPorIdQuery
        //    return Ok(); // placeholder
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<CaixaDto>>> Listar()
        //{
        //    // Implemente a query: ListarCaixasQuery
        //    return Ok(); // placeholder
        //}
    }
}
