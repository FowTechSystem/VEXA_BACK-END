using Microsoft.AspNetCore.Mvc;
using MediatR;


namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedoresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FornecedoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<ActionResult<int>> Criar([FromBody] CriarFornecedorCommand command)
        //{
        //    var id = await _mediator.Send(command);
        //    return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<FornecedorDto>> ObterPorId(int id)
        //{
        //    // Implemente a query: ObterFornecedorPorIdQuery
        //    return Ok(); // placeholder
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<FornecedorDto>>> Listar()
        //{
        //    // Implemente a query: ListarFornecedoresQuery
        //    return Ok(); // placeholder
        //}
    }
}
