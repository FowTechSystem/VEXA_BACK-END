using CRMVEXA.Application.Features.Clientes.CriarCliente;
using CRMVEXA.Application.Features.Clientes.DeleteCliente;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<ActionResult<int>> Criar([FromBody] CriarClienteRequest request)
        {
            try
            {
                var id = await _mediator.Send(request);
                return Ok(new { sucesso = true, id, mensagem = "Cliente criado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar([FromRoute] int id)
        {
            try
            {
                var sucesso = await _mediator.Send(new DeleteClienteRequest { id = id });

                if(sucesso is null)
                    return NotFound(new { sucesso = false, mensagem = "Cliente não encontrado." }); 

                return Convert.ToBoolean(sucesso)
                    ? Ok(new { sucesso = true, mensagem = "Cliente deletado!" })
                    : NotFound(new { sucesso = false, mensagem = "Cliente não encontrado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = ex.Message });
            }
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<ClienteDto>> ObterPorId(int id)
        //{
        //    // Implemente a query: ObterClientePorIdQuery
        //    // Exemplo:
        //    // var cliente = await _mediator.Send(new ObterClientePorIdQuery(id));
        //    // return Ok(cliente);
        //    return Ok(); // placeholder
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ClienteDto>>> Listar()
        //{
        //    // Implemente a query: ListarClientesQuery
        //    // Exemplo:
        //    // var clientes = await _mediator.Send(new ListarClientesQuery());
        //    // return Ok(clientes);
        //    return Ok(); // placeholder
        //}
    }
}
