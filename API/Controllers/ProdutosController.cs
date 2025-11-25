using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRMVEXA.Application.Commands.Produtos;
using CRMVEXA.Application.DTOs;

namespace CRMVEXA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Criar([FromBody] CriarProdutoCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDto>> ObterPorId(int id)
        {
            // Implemente a query: ObterProdutoPorIdQuery
            return Ok(); // placeholder
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> Listar()
        {
            // Implemente a query: ListarProdutosQuery
            return Ok(); // placeholder
        }
    }
}
